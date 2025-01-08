using NTDLS.KitKey.Shared;
using NTDLS.Semaphore;
using RocksDbSharp;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// A named message queue and its delivery thread.
    /// </summary>
    internal class MessageQueue
    {
        private readonly KkServer _keyServer;

        /// <summary>
        /// Messages that are enqueued in this list.
        /// </summary>
        internal OptimisticCriticalResource<RocksDb>? Database { get; set; }

        internal KkStoreConfiguration Configuration { get; set; }

        internal MessageQueueStatistics Statistics { get; set; } = new();

        public MessageQueue(KkServer keyServer, KkStoreConfiguration queueConfiguration)
        {
            _keyServer = keyServer;
            Configuration = queueConfiguration;
        }

        public void Start()
        {
            if (Configuration.PersistenceScheme != CMqPersistenceScheme.Persistent)
            {
                return;
            }

            _keyServer.InvokeOnLog(CMqErrorLevel.Verbose, $"Creating persistent path for [{Configuration.StoreName}].");

            var databasePath = Path.Join(_keyServer.Configuration.PersistencePath, "store", Configuration.StoreName);
            Directory.CreateDirectory(databasePath);

            _keyServer.InvokeOnLog(CMqErrorLevel.Information, $"Instantiating persistent key-store for [{Configuration.StoreName}].");
            var options = new DbOptions().SetCreateIfMissing(true);
            var persistenceDatabase = RocksDb.Open(options, databasePath);

            Database = new(persistenceDatabase);
        }

        public void Stop()
        {
            _keyServer.InvokeOnLog(CMqErrorLevel.Information, $"Signaling shutdown for [{Configuration.StoreName}].");

            Database?.Write(o =>
            {
                o.Checkpoint();
                o.Dispose();
            });
        }

        public void Upsert(string key, string value)
        {
            /*
            success = messageQueue.Database.TryWrite(CMqDefaults.DEFAULT_TRY_WAIT_MS, m =>
            {
                messageQueue.Statistics.ReceivedMessageCount++;
                var message = new EnqueuedMessage(queueKey, assemblyQualifiedTypeName, messageJson);
                if (messageQueue.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent && m.Database != null)
                {
                    //Serialize using System.Text.Json as opposed to Newtonsoft for efficiency.
                    var persistedJson = JsonSerializer.Serialize(message);
                    m.Database?.Put(message.MessageId.ToString(), persistedJson);
                }

                m.Messages.Add(message);
                messageQueue.DeliveryThreadWaitEvent.Set();
            }) && success;
            */
        }

        public void Purge()
        {
            /*
                        while (_keepRunning)
                        {
                            bool success = true;

                            _messageQueues.Read(mqd =>
                            {
                                string queueKey = queueName.ToLowerInvariant();
                                if (mqd.TryGetValue(queueKey, out var messageQueue))
                                {
                                    success = messageQueue.EnqueuedMessages.TryWrite(CMqDefaults.DEFAULT_TRY_WAIT_MS, m =>
                                    {
                                        if (messageQueue.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent && m.Database != null)
                                        {
                                            foreach (var message in m.Messages)
                                            {
                                                m.Database?.Remove(message.MessageId.ToString());
                                            }
                                        }
                                        m.Messages.Clear();
                                    }) && success;
                                }
                                else
                                {
                                    throw new Exception($"Queue not found: [{queueName}].");
                                }
                            });

                            if (success)
                            {
                                return;
                            }
                            Thread.Sleep(CMqDefaults.DEFAULT_DEADLOCK_AVOIDANCE_WAIT_MS);
                        }
             */

        }
    }
}
