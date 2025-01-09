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
        private readonly OptimisticCriticalResource<CaseInsensitiveKeyStoreDictionary> _keyStores = new();
        private readonly RmServer _rmServer;

        internal KkServerConfiguration Configuration => _configuration;

        /// <summary>
        /// Delegate used to notify of key-store server exceptions.
        /// </summary>
        public delegate void OnLogEvent(KkServer server, KkErrorLevel errorLevel, string message, Exception? ex = null);

        /// <summary>
        /// Event used to notify of key-store server exceptions.
        /// </summary>
        public event OnLogEvent? OnLog;

        /// <summary>
        /// Creates a new instance of the key-store service.
        /// </summary>
        public KkServer(KkServerConfiguration configuration)
        {
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
                OnLog?.Invoke(this, KkErrorLevel.Verbose, "Checkpoint persistent key-stores.");

                var storeMetas = mqd.Where(q => q.Value.Configuration.PersistenceScheme == KkPersistenceScheme.Persistent)
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

            _keyStores.Read(ks =>
            {
                foreach (var ksKPV in ks)
                {
                    result.Add(new KkStoreDescriptor
                    {
                        CurrentValueCount = ksKPV.Value.CurrentValueCount(),
                        CacheHits = ksKPV.Value.Statistics.CacheHits,
                        CacheMisses = ksKPV.Value.Statistics.CacheMisses,
                        DatabaseHits = ksKPV.Value.Statistics.DatabaseHits,
                        DatabaseMisses = ksKPV.Value.Statistics.DatabaseMisses,
                        DeleteCount = ksKPV.Value.Statistics.DeleteCount,
                        GetCount = ksKPV.Value.Statistics.GetCount,
                        PersistenceScheme = ksKPV.Value.Configuration.PersistenceScheme,
                        CacheExpiration = ksKPV.Value.Configuration.CacheExpiration,
                        SetCount = ksKPV.Value.Statistics.SetCount,
                        StoreName = ksKPV.Value.Configuration.StoreName,
                    });
                }
            });

            return new ReadOnlyCollection<KkStoreDescriptor>(result);
        }

        #endregion

        internal void InvokeOnLog(Exception ex)
            => OnLog?.Invoke(this, KkErrorLevel.Error, ex.Message, ex);

        internal void InvokeOnLog(KkErrorLevel errorLevel, string message)
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
                    OnLog?.Invoke(this, KkErrorLevel.Information, "Loading persistent key-stores.");

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

            OnLog?.Invoke(this, KkErrorLevel.Information, "Starting key-stores.");
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
            OnLog?.Invoke(this, KkErrorLevel.Information, "Stopping service.");

            _keepRunning = false;
            OnLog?.Invoke(this, KkErrorLevel.Information, "Stopping reliable messaging.");
            _rmServer.Stop();

            var keyStores = new List<KeyStore>();

            _keyStores.Read(mqd =>
            {
                //Stop all key stores.
                foreach (var mqKVP in mqd)
                {
                    OnLog?.Invoke(this, KkErrorLevel.Information, $"Stopping key-store [{mqKVP.Value.Configuration.StoreName}].");
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

        /// <summary>
        /// Returns a reference to a key-store for read.
        /// Read here means that we will not modify the collection of key stores, but we are
        /// free to add/remove values as their concurrency is handled by the key-store itself.
        /// </summary>
        private KeyStore GetKeyStore(string storeName)
        {
            var storeKey = storeName.ToLowerInvariant();

            return _keyStores.Read(mqd =>
            {
                if (mqd.TryGetValue(storeKey, out var store))
                {
                    return store;
                }
                throw new Exception($"Key-store not found: [{storeName}].");
            });
        }

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

            _keyStores.Write(mqd =>
            {
                string storeKey = storeConfiguration.StoreName.ToLowerInvariant();
                if (mqd.ContainsKey(storeKey) == false)
                {
                    var keyStore = new KeyStore(this, storeConfiguration);
                    mqd.Add(storeKey, keyStore);

                    if (storeConfiguration.PersistenceScheme == Shared.KkPersistenceScheme.Persistent)
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
            string storeKey = storeName.ToLowerInvariant();

            while (_keepRunning)
            {
                KeyStore? keyStore = null;

                _keyStores.Write(mqd =>
                {
                    if (mqd.TryGetValue(storeKey, out var store))
                    {
                        keyStore = store;
                        store.Stop();
                        mqd.Remove(storeKey);
                        CheckpointPersistentStores(mqd);
                    }
                });

                if (keyStore != null)
                {
                    var databasePath = Path.Join(Configuration.PersistencePath, "store", keyStore.Configuration.StoreName);

                    try
                    {
                        Directory.Delete(databasePath, true);
                    }
                    catch (Exception ex)
                    {
                        OnLog?.Invoke(this, KkErrorLevel.Verbose, $"Failed to delete persisted key-store values for [{storeName}].", ex);
                    }
                }
                return;
            }
        }

        #region String.

        /// <summary>
        /// Inserts/updates a value in the key-store.
        /// </summary>
        public void SetString(string storeName, string key, string value)
            => GetKeyStore(storeName)?.SetString(key, value);

        /// <summary>
        /// Gets a value by its key form the key-store.
        /// </summary>
        public string? GetString(string storeName, string key)
            => GetKeyStore(storeName)?.GetString(key);

        #endregion

        #region List.

        /// <summary>
        /// Appends a value to a list key-store.
        /// </summary>
        public void AppendList(string storeName, string key, string value)
            => GetKeyStore(storeName)?.AppendList(key, value);

        /// <summary>
        /// Gets a list from the key-store by its key.
        /// </summary>
        public List<KkListItem>? GetList(string storeName, string key)
            => GetKeyStore(storeName)?.GetList(key);

        #endregion

        /// <summary>
        /// Removes a value form the key-store.
        /// </summary>
        public void Delete(string storeName, string key)
            => GetKeyStore(storeName)?.Delete(key);

        /// <summary>
        /// Removes all messages from the given key-store.
        /// </summary>
        public void PurgeStore(string storeName)
            => GetKeyStore(storeName)?.Purge();

        #endregion
    }
}
