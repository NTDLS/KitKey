﻿using NTDLS.KitKey.Server.QueryHandlers;
using NTDLS.KitKey.Server.Server;
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
                InitialReceiveBufferSize = configuration.InitialReceiveBufferSize,
                MaxReceiveBufferSize = configuration.MaxReceiveBufferSize,
                QueryTimeout = TimeSpan.FromSeconds(configuration.AcknowledgmentTimeoutSeconds),
                ReceiveBufferGrowthRate = configuration.ReceiveBufferGrowthRate,
            };

            _rmServer = new RmServer(rmConfiguration);
            _rmServer.SetCompressionProvider(new RmDeflateCompressionProvider());

            _rmServer.AddHandler(new RemovesQueryHandler(this));
            _rmServer.AddHandler(new StoresQueryHandler(this));

            _rmServer.AddHandler(new SingleValueQueryHandlers<string>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int32?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int32>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int64?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int64>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Single?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Single>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Double?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Double>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<DateTime?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<DateTime>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Guid?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Guid>(this));

            _rmServer.AddHandler(new ListsQueryHandlers<string>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int32?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int32>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int64?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int64>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Single?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Single>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Double?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Double>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<DateTime?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<DateTime>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Guid?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Guid>(this));
        }

        /// <summary>
        /// Creates a new instance of the key-store service.
        /// </summary>
        public KkServer()
        {
            _configuration = new KkServerConfiguration();
            _rmServer = new RmServer();
            _rmServer.SetCompressionProvider(new RmDeflateCompressionProvider());

            _rmServer.AddHandler(new RemovesQueryHandler(this));
            _rmServer.AddHandler(new StoresQueryHandler(this));

            _rmServer.AddHandler(new SingleValueQueryHandlers<string>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int32?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int32>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int64?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Int64>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Single?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Single>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Double?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Double>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<DateTime?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<DateTime>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Guid?>(this));
            _rmServer.AddHandler(new SingleValueQueryHandlers<Guid>(this));

            _rmServer.AddHandler(new ListsQueryHandlers<string>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int32?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int32>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int64?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Int64>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Single?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Single>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Double?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Double>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<DateTime?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<DateTime>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Guid?>(this));
            _rmServer.AddHandler(new ListsQueryHandlers<Guid>(this));
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
                        ValueType = ksKPV.Value.Configuration.ValueType,
                        CacheExpiration = ksKPV.Value.Configuration.CacheExpiration,
                        SetCount = ksKPV.Value.Statistics.SetCount,
                        StoreKey = ksKPV.Value.Configuration.StoreKey,
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
                                mqd.Add(storeMeta.Configuration.StoreKey, keyStore);
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
                    OnLog?.Invoke(this, KkErrorLevel.Information, $"Stopping key-store [{mqKVP.Value.Configuration.StoreKey}].");
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
        internal KeyStore GetKeyStore(string storeKey)
        {
            return _keyStores.Read(mqd =>
            {
                if (mqd.TryGetValue(storeKey, out var store))
                {
                    return store;
                }
                throw new Exception($"Key-store not found: [{storeKey}].");
            });
        }

        #region Client interactions.

        /// <summary>
        /// Creates a new empty key-store if it does not already exist.
        /// </summary>
        public void CreateStore(KkStoreConfiguration storeConfiguration)
        {
            if (string.IsNullOrEmpty(storeConfiguration.StoreKey))
            {
                throw new Exception("A key-store name is required.");
            }

            _keyStores.Write(mqd =>
            {
                string storeKey = storeConfiguration.StoreKey;
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
        /// Clears the cache for a single key-store.
        /// </summary>
        public void FlushCache(string storeKey)
        {
            _keyStores.Read(mqd =>
             {
                 if (mqd.TryGetValue(storeKey, out var keyStore))
                 {
                     keyStore.FlushCache();
                 }
             });
        }

        /// <summary>
        /// Clears the cache for a all key-stores.
        /// </summary>
        public void FlushCache()
        {
            _keyStores.Read(mqd =>
            {
                foreach (var keystore in mqd.Values)
                {
                    keystore.FlushCache();
                }
            });
        }

        /// <summary>
        /// Deletes an existing key-store.
        /// </summary>
        public void DeleteStore(string storeKey)
        {
            var keyStore = _keyStores.Write(mqd =>
            {
                if (mqd.TryGetValue(storeKey, out var keyStore))
                {
                    keyStore.Stop();
                    mqd.Remove(storeKey);
                    CheckpointPersistentStores(mqd);
                }
                return keyStore;
            });

            if (keyStore != null)
            {
                var databasePath = Path.Join(Configuration.PersistencePath, "store", keyStore.Configuration.StoreKey);

                try
                {
                    Directory.Delete(databasePath, true);
                }
                catch (Exception ex)
                {
                    OnLog?.Invoke(this, KkErrorLevel.Verbose, $"Failed to delete persisted key-store values for [{storeKey}].", ex);
                }
            }
        }

        /// <summary>
        /// Removes a key from a key-store of any type.
        /// </summary>
        public void RemoveKey(string storeKey, string valueKey)
            => GetKeyStore(storeKey).DeleteKey(valueKey);

        /// <summary>
        /// Removes a key from a key-store of any type.
        /// </summary>
        public void RemoveListItemByKey(string storeKey, string valueKey, Guid listItemKey)
            => GetKeyStore(storeKey).DeleteListItemByKey(valueKey, listItemKey);

        /// <summary>
        /// Removes all messages from the given key-store.
        /// </summary>
        public void PurgeStore(string storeKey)
            => GetKeyStore(storeKey).StorePurge();

        /// <summary>
        /// Inserts/updates a value in the key-store.
        /// </summary>
        public void SetSingleValue<T>(string storeKey, string key, T value)
            => GetKeyStore(storeKey).SetSingleValue<T>(key, value);

        /// <summary>
        /// Gets a value by its key form the key-store.
        /// </summary>
        public T? GetSingleValue<T>(string storeKey, string valueKey)
            => GetKeyStore(storeKey).GetSingleValue<T>(valueKey);

        #region Get/Set List.

        /// <summary>
        /// Prepends a value to a list key-store.
        /// </summary>
        public void PushFirst<T>(string storeKey, string valueKey, T value)
            => GetKeyStore(storeKey).PushFirst(valueKey, value);

        /// <summary>
        /// Appends a value to a list key-store.
        /// </summary>
        public void PushLast<T>(string storeKey, string valueKey, T value)
            => GetKeyStore(storeKey).PushLast(valueKey, value);


        /// <summary>
        /// Prepends a value to a list key-store.
        /// </summary>
        public void PushFirst<T>(string storeKey, string valueKey, KkListItem<T> item)
            => GetKeyStore(storeKey).PushFirst(valueKey, item);

        /// <summary>
        /// Appends a value to a list key-store.
        /// </summary>
        public void PushLast<T>(string storeKey, string valueKey, KkListItem<T> item)
            => GetKeyStore(storeKey).PushLast(valueKey, item);

        /// <summary>
        /// Gets a list from the key-store by its key.
        /// </summary>
        public List<KkListItem<T>>? GetList<T>(string storeKey, string valueKey)
            => GetKeyStore(storeKey).GetList<T>(valueKey);

        /// <summary>
        /// Gets the item at the bottom of the list in the key-store.
        /// </summary>
        public KkListItem<T>? GetFirst<T>(string storeKey, string valueKey)
            => GetKeyStore(storeKey).GetFirst<T>(valueKey);

        /// <summary>
        /// Gets the item at the top of the list in the key-store.
        /// </summary>
        public KkListItem<T>? GetLast<T>(string storeKey, string valueKey)
            => GetKeyStore(storeKey).GetLast<T>(valueKey);

        #endregion

        #endregion
    }
}
