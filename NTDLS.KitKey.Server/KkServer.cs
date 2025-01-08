﻿using NTDLS.KitKey.Server.Management;
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
        private readonly OptimisticCriticalResource<CaseInsensitiveKeyStoreDictionary> _keyStores = new();
        private readonly RmServer _rmServer;

        internal KkServerConfiguration Configuration => _configuration;

        /// <summary>
        /// Delegate used to notify of key-store server exceptions.
        /// </summary>
        public delegate void OnLogEvent(KkServer server, CMqErrorLevel errorLevel, string message, Exception? ex = null);

        /// <summary>
        /// Event used to notify of key-store server exceptions.
        /// </summary>
        public event OnLogEvent? OnLog;

        /// <summary>
        /// Creates a new instance of the key-store service.
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
        /// Creates a new instance of the key-store service.
        /// </summary>
        public KkServer()
        {
            _configuration = new KkServerConfiguration();
            _rmServer = new RmServer();
            _rmServer.AddHandler(new InternalServerQueryHandlers(this));
        }

        #region Management.

        /// <summary>
        /// Saves persistent key stores and their statistics to disk.
        /// </summary>
        public void CheckpointPersistentStores()
        {
            _keyStores.Read(mqd => CheckpointPersistentStores(mqd));
        }

        private void CheckpointPersistentStores(CaseInsensitiveKeyStoreDictionary mqd)
        {
            if (string.IsNullOrEmpty(_configuration.PersistencePath) == false)
            {
                OnLog?.Invoke(this, CMqErrorLevel.Verbose, "Checkpoint persistent key-stores.");

                var storeMetas = mqd.Where(q => q.Value.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent)
                    .Select(q => new KeyStoreMetadata(q.Value.Configuration, q.Value.Statistics)).ToList();

                //Serialize using System.Text.Json as opposed to Newtonsoft for efficiency.
                var persistedStoresJson = JsonSerializer.Serialize(storeMetas, _indentedJsonOptions);
                File.WriteAllText(Path.Join(_configuration.PersistencePath, "stores.json"), persistedStoresJson);
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
        /// Returns a read-only copy of the key-stores.
        /// </summary>
        public ReadOnlyCollection<KkStoreDescriptor>? GetStores()
        {
            List<KkStoreDescriptor>? result = new();

            _keyStores.Read(mqd =>
            {
                foreach (var mqKVP in mqd)
                {
                    result.Add(new KkStoreDescriptor
                    {
                        PersistenceScheme = mqKVP.Value.Configuration.PersistenceScheme,
                        StoreName = mqKVP.Value.Configuration.StoreName,

                        //TODO: CurrentMessageCount = m.Messages.Count,

                        UpsertCount = mqKVP.Value.Statistics.UpsertCount,
                        GetCount = mqKVP.Value.Statistics.GetCount,
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
        /// Starts the key store server.
        /// </summary>
        public void Start(int listenPort)
        {
            if (_keepRunning)
            {
                return;
            }

            _keepRunning = true;

            var storesToStart = new List<KeyStore>();

            if (_configuration.PersistencePath != null)
            {
                var persistedStoresFile = Path.Join(_configuration.PersistencePath, "stores.json");
                if (File.Exists(persistedStoresFile))
                {
                    OnLog?.Invoke(this, CMqErrorLevel.Information, "Loading persistent key-stores.");

                    var keyStoresJson = File.ReadAllText(persistedStoresFile);
                    //Deserialize using System.Text.Json as opposed to Newtonsoft for efficiency.
                    var storesMeta = JsonSerializer.Deserialize<List<KeyStoreMetadata>>(keyStoresJson);

                    if (storesMeta != null)
                    {
                        _keyStores.Write(mqd =>
                        {
                            foreach (var storeMeta in storesMeta)
                            {
                                var keyStore = new KeyStore(this, storeMeta.Configuration)
                                {
                                    Statistics = storeMeta.Statistics
                                };
                                storesToStart.Add(keyStore);
                                mqd.Add(storeMeta.Configuration.StoreName.ToLowerInvariant(), keyStore);
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
        /// Stops the key store server.
        /// </summary>
        public void Stop()
        {
            OnLog?.Invoke(this, CMqErrorLevel.Information, "Stopping service.");

            _keepRunning = false;
            OnLog?.Invoke(this, CMqErrorLevel.Information, "Stopping reliable messaging.");
            _rmServer.Stop();

            var keyStores = new List<KeyStore>();

            _keyStores.Read(mqd =>
            {
                //Stop all key stores.
                foreach (var mqKVP in mqd)
                {
                    OnLog?.Invoke(this, CMqErrorLevel.Information, $"Stopping key-store [{mqKVP.Value.Configuration.StoreName}].");
                    mqKVP.Value.Stop();
                    keyStores.Add(mqKVP.Value);
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
        /// Creates a new empty key-store if it does not already exist.
        /// </summary>
        public void CreateStore(KkStoreConfiguration storeConfiguration)
        {
            if (string.IsNullOrEmpty(storeConfiguration.StoreName))
            {
                throw new Exception("A key-store name is required.");
            }

            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Creating key-store: [{storeConfiguration.StoreName}].");

            _keyStores.Write(mqd =>
            {
                string storeKey = storeConfiguration.StoreName.ToLowerInvariant();
                if (mqd.ContainsKey(storeKey) == false)
                {
                    var keyStore = new KeyStore(this, storeConfiguration);
                    mqd.Add(storeKey, keyStore);

                    if (storeConfiguration.PersistenceScheme == CMqPersistenceScheme.Persistent)
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

                    keyStore.Start();
                }
            });
        }

        /// <summary>
        /// Deletes an existing key-store.
        /// </summary>
        public void DeleteStore(string storeName)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Deleting key-store: [{storeName}].");

            string storeKey = storeName.ToLowerInvariant();

            while (_keepRunning)
            {
                bool success = true;

                KeyStore? cleanupStore = null;

                _keyStores.Write(mqd =>
                {
                    if (mqd.TryGetValue(storeKey, out var store))
                    {
                        store.Stop();

                        if (store.Configuration.PersistenceScheme == CMqPersistenceScheme.Persistent && store.Database != null)
                        {
                            success = store.Database.TryWrite(KkDefaults.DEFAULT_TRY_WAIT_MS, m =>
                            {
                                cleanupStore = store;
                                mqd.Remove(storeKey);
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
                            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Failed to delete persisted key-store values for [{storeName}].", ex);
                        }
                    }
                    return;
                }
                Thread.Sleep(KkDefaults.DEFAULT_DEADLOCK_AVOIDANCE_WAIT_MS);
            }
        }

        /// <summary>
        /// Inserts/updates a value in the key-store.
        /// </summary>
        public void Upsert(string storeName, string key, string value)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Storing value to key-store: [{storeName}].");

            string storeKey = storeName.ToLowerInvariant();

            _keyStores.Read(mqd =>
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
        /// Removes all messages from the given key-store.
        /// </summary>
        public void PurgeStore(string storeName)
        {
            OnLog?.Invoke(this, CMqErrorLevel.Verbose, $"Purging key-store: [{storeName}].");

            string storeKey = storeName.ToLowerInvariant();

            _keyStores.Read(mqd =>
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
