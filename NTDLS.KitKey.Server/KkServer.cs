using NTDLS.KitKey.Server.Management;
using NTDLS.KitKey.Server.Server;
using NTDLS.KitKey.Server.Server.QueryHandlers;
using NTDLS.KitKey.Shared;
using NTDLS.ReliableMessaging;
using NTDLS.Semaphore;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace NTDLS.KitKey.Server
{
    /// <summary>
    /// Listens for connections from MessageClients and processes the incoming notifications/queries.
    /// </summary>
    public class KkServer
    {
        private bool _keepRunning = false;
        private readonly KkServerConfiguration _configuration;
        private readonly JsonSerializerOptions _indentedJsonOptions = new() { WriteIndented = true };
        private readonly OptimisticCriticalResource<CaseInsensitiveMessageQueueDictionary> _messageQueues = new();
        private readonly RmServer _rmServer;

        internal KkServerConfiguration Configuration => _configuration;

        /// <summary>
        /// Delegate used to notify of queue server exceptions.
        /// </summary>
        public delegate void OnLogEvent(KkServer server, CMqErrorLevel errorLevel, string message, Exception? ex = null);

        /// <summary>
        /// Event used to notify of queue server exceptions.
        /// </summary>
        public event OnLogEvent? OnLog;

        /// <summary>
        /// Creates a new instance of the queue service.
        /// </summary>
        public KkServer(KkServerConfiguration configuration)
        {
            //ThreadLockOwnershipTracking.Enable();

            _configuration = configuration;

            var rmConfiguration = new RmConfiguration()
            {
                AsynchronousQueryWaiting = configuration.AsynchronousAcknowledgment,
                InitialReceiveBufferSize = configuration.InitialReceiveBufferSize,
                MaxReceiveBufferSize = configuration.MaxReceiveBufferSize,
                QueryTimeout = TimeSpan.FromSeconds(configuration.AcknowledgmentTimeoutSeconds),
                ReceiveBufferGrowthRate = configuration.ReceiveBufferGrowthRate,
            };

            _rmServer = new RmServer(rmConfiguration);
            _rmServer.AddHandler(new InternalServerQueryHandlers(this));
        }

        /// <summary>
        /// Creates a new instance of the queue service.
        /// </summary>
        public KkServer()
        {
            _configuration = new KkServerConfiguration();
            _rmServer = new RmServer();
            _rmServer.AddHandler(new InternalServerQueryHandlers(this));
        }

        #region Management.

        /// <summary>
        /// Saves persistent message queues and their statistics to disk.
        /// </summary>
        public void CheckpointPersistentStores()
        {
            _messageQueues.Read(mqd => CheckpointPersistentStores(mqd));
        }

        private void CheckpointPersistentStores(CaseInsensitiveMessageQueueDictionary mqd)
        {
            if (string.IsNullOrEmpty(_configuration.PersistencePath) == false)
            {
                OnLog?.Invoke(this, CMqErrorLevel.Verbose, "Checkpoint persistent queues.");

                var queueMetas = mqd.Where(q => q.Value.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent)
                    .Select(q => new MessageQueueMetadata(q.Value.Configuration, q.Value.Statistics)).ToList();

                //Serialize using System.Text.Json as opposed to Newtonsoft for efficiency.
                var persistedQueuesJson = JsonSerializer.Serialize(queueMetas, _indentedJsonOptions);
                File.WriteAllText(Path.Join(_configuration.PersistencePath, "stores.json"), persistedQueuesJson);
            }
        }

        /// <summary>
        /// Returns a read-only copy of the running configuration.
        /// </summary>
        /// <returns></returns>
        public KkServerDescriptor GetConfiguration()
        {
            return new KkServerDescriptor
            {
                AsynchronousAcknowledgment = _configuration.AsynchronousAcknowledgment,
                AcknowledgmentTimeoutSeconds = _configuration.AcknowledgmentTimeoutSeconds,
                InitialReceiveBufferSize = _configuration.InitialReceiveBufferSize,
                MaxReceiveBufferSize = _configuration.MaxReceiveBufferSize,
                ReceiveBufferGrowthRate = _configuration.ReceiveBufferGrowthRate,
                ListenPort = _rmServer.ListenPort,
                PersistencePath = _configuration.PersistencePath
            };
        }

        /// <summary>
        /// Returns a read-only copy of the queues.
        /// </summary>
        public ReadOnlyCollection<KkStoreDescriptor>? GetStores()
        {
            List<KkStoreDescriptor>? result = new();

            _messageQueues.Read(mqd =>
            {
                foreach (var mqKVP in mqd)
                {
                    result.Add(new KkStoreDescriptor
                    {
                        PersistenceScheme = mqKVP.Value.Configuration.PersistenceScheme,
                        StoreName = mqKVP.Value.Configuration.StoreName,

                        //TODO: CurrentMessageCount = m.Messages.Count,

                        ReceivedMessageCount = mqKVP.Value.Statistics.ReceivedMessageCount,
                        DeliveredMessageCount = mqKVP.Value.Statistics.DeliveredMessageCount,
                    });
                }
            });

            return new ReadOnlyCollection<KkStoreDescriptor>(result);
        }

        #endregion

        internal void InvokeOnLog(Exception ex)
            => OnLog?.Invoke(this, CMqErrorLevel.Error, ex.Message, ex);

        internal void InvokeOnLog(CMqErrorLevel errorLevel, string message)
            => OnLog?.Invoke(this, errorLevel, message);

        #region Start & Stop.

        /// <summary>
        /// Starts the message queue server.
        /// </summary>
        public void Start(int listenPort)
        {
            if (_keepRunning)
            {
                return;
            }

            _keepRunning = true;

            var storesToStart = new List<MessageQueue>();

            if (_configuration.PersistencePath != null)
            {
                var persistedStoresFile = Path.Join(_configuration.PersistencePath, "stores.json");
                if (File.Exists(persistedStoresFile))
                {
                    OnLog?.Invoke(this, CMqErrorLevel.Information, "Loading persistent key-stores.");

                    var persistedQueuesJson = File.ReadAllText(persistedStoresFile);
                    //Deserialize using System.Text.Json as opposed to Newtonsoft for efficiency.
                    var storesMeta = JsonSerializer.Deserialize<List<MessageQueueMetadata>>(persistedQueuesJson);

                    if (storesMeta != null)
                    {
                        _messageQueues.Write(mqd =>
                        {
                            foreach (var storeMeta in storesMeta)
                            {
                                var messageQueue = new MessageQueue(this, storeMeta.Configuration)
                                {
                                    Statistics = storeMeta.Statistics
                                };
                                storesToStart.Add(messageQueue);
                                mqd.Add(storeMeta.Configuration.StoreName.ToLowerInvariant(), messageQueue);
                            }
                        });
                    }
                }
            }

            OnLog?.Invoke(this, CMqErrorLevel.Information, "Starting key-stores.");
            foreach (var store in storesToStart)
            {
                store.Start();
            }

            _rmServer.Start(listenPort);

            new Thread(() => HeartbeatThread()).Start();
        }

        private void HeartbeatThread()
        {
            var lastCheckpoint = DateTime.UtcNow;

            while (_keepRunning)
            {
                if (DateTime.UtcNow - lastCheckpoint > TimeSpan.FromSeconds(30))
                {
                    CheckpointPersistentStores();
                    lastCheckpoint = DateTime.UtcNow;
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Stops the message queue server.
        /// </summary>
        public void Stop()
        {
            OnLog?.Invoke(this, CMqErrorLevel.Information, "Stopping service.");

            _keepRunning = false;
            OnLog?.Invoke(this, CMqErrorLevel.Information, "Stopping reliable messaging.");
            _rmServer.Stop();

            var messageQueues = new List<MessageQueue>();

            _messageQueues.Read(mqd =>
            {
                //Stop all message queues.
                foreach (var mqKVP in mqd)
                {
                    OnLog?.Invoke(this, CMqErrorLevel.Information, $"Stopping queue [{mqKVP.Value.Configuration.StoreName}].");
                    mqKVP.Value.Stop();
                    messageQueues.Add(mqKVP.Value);
                }

                if (string.IsNullOrEmpty(_configuration.PersistencePath) == false)
                {
                    CheckpointPersistentStores(mqd);
                }
            });
        }

        #endregion

        #region Client interactions.

        /// <summary>
        /// Creates a new empty queue if it does not already exist.
        /// </summary>
        public void CreateStore(KkStoreConfiguration queueConfiguration)
        {
            if (string.IsNullOrEmpty(queueConfiguration.StoreName))
            {
                throw new Exception("A queue name is required.");
            }

            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Creating queue: [{queueConfiguration.StoreName}].");

            _messageQueues.Write(mqd =>
            {
                string queueKey = queueConfiguration.StoreName.ToLowerInvariant();
                if (mqd.ContainsKey(queueKey) == false)
                {
                    var messageQueue = new MessageQueue(this, queueConfiguration);
                    mqd.Add(queueKey, messageQueue);

                    if (queueConfiguration.PersistenceScheme == CMqPersistenceScheme.Persistent)
                    {
                        if (string.IsNullOrEmpty(_configuration.PersistencePath) == false)
                        {
                            CheckpointPersistentStores(mqd);
                        }
                        else
                        {
                            throw new Exception("The server persistence path is not configured.");
                        }
                    }

                    messageQueue.Start();
                }
            });
        }

        /// <summary>
        /// Deletes an existing queue.
        /// </summary>
        public void DeleteStore(string queueName)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Deleting queue: [{queueName}].");

            string queueKey = queueName.ToLowerInvariant();

            while (_keepRunning)
            {
                bool success = true;

                MessageQueue? cleanupStore = null;

                _messageQueues.Write(mqd =>
                {
                    if (mqd.TryGetValue(queueKey, out var messageQueue))
                    {
                        messageQueue.Stop();

                        if (messageQueue.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent && messageQueue.Database != null)
                        {
                            success = messageQueue.Database.TryWrite(KkDefaults.DEFAULT_TRY_WAIT_MS, m =>
                            {
                                cleanupStore = messageQueue;
                                mqd.Remove(queueKey);
                            }) && success;
                        }

                        if (success)
                        {
                            if (string.IsNullOrEmpty(_configuration.PersistencePath) == false)
                            {
                                CheckpointPersistentStores(mqd);
                            }
                        }
                    }
                });

                if (success)
                {
                    if (cleanupStore != null)
                    {
                        var databasePath = Path.Join(Configuration.PersistencePath, "store", cleanupStore.Configuration.StoreName);

                        try
                        {
                            Directory.Delete(databasePath, true);
                        }
                        catch (Exception ex)
                        {
                            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Failed to delete persisted queue messages for [{queueName}].", ex);
                        }
                    }
                    return;
                }
                Thread.Sleep(KkDefaults.DEFAULT_DEADLOCK_AVOIDANCE_WAIT_MS);
            }
        }

        /// <summary>
        /// Removes a subscription from a queue for a given connection id.
        /// </summary>
        public void Upsert(string storeName, string key, string value)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Storing value to key-store: [{storeName}].");

            string storeKey = storeName.ToLowerInvariant();

            _messageQueues.Read(mqd =>
            {
                if (mqd.TryGetValue(storeKey, out var store))
                {
                    store.Upsert(key, value);
                }
                else
                {
                    throw new Exception($"Key-store not found: [{storeName}].");
                }
            });
        }

        /// <summary>
        /// Removes all messages from the given queue.
        /// </summary>
        public void PurgeStore(string storeName)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Purging key-store: [{storeName}].");

            string storeKey = storeName.ToLowerInvariant();

            _messageQueues.Read(mqd =>
            {
                if (mqd.TryGetValue(storeKey, out var store))
                {
                    store.Purge();
                }
                else
                {
                    throw new Exception($"Key-store not found: [{storeName}].");
                }
            });
        }

        #endregion
    }
}
