using NTDLS.FastMemoryCache;
using NTDLS.KitKey.Shared;
using NTDLS.Semaphore;
using RocksDbSharp;
using System.Text.Json;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// A named key store, its cache and persistence database.
    /// </summary>
    internal class KeyStore
    {
        private readonly ConcurrentKeyOperation _concurrentKeyOperation = new();
        private readonly KkServer _keyServer;
        private OptimisticCriticalResource<RocksDb>? _database;
        private readonly PartitionedMemoryCache _memoryCache;

        internal KkStoreConfiguration Configuration { get; private set; }
        internal KeyStoreStatistics Statistics { get; set; } = new();

        public KeyStore(KkServer keyServer, KkStoreConfiguration storeConfiguration)
        {
            _keyServer = keyServer;
            Configuration = storeConfiguration;
            _memoryCache = new PartitionedMemoryCache();

            if (Configuration.CacheExpiration == null)
            {
                if (Configuration.PersistenceScheme == KkPersistenceScheme.Persistent)
                {
                    Configuration.CacheExpiration = TimeSpan.FromSeconds(KkDefaults.DEFAULT_CACHE_SECONDS);
                }
                else if (Configuration.PersistenceScheme == KkPersistenceScheme.Ephemeral)
                {
                    Configuration.CacheExpiration = TimeSpan.FromDays(1);
                }
            }
        }

        public void Start()
        {
            if (Configuration.PersistenceScheme != KkPersistenceScheme.Persistent)
            {
                return;
            }

            _keyServer.InvokeOnLog(KkErrorLevel.Verbose, $"Creating persistent path for [{Configuration.StoreKey}].");

            var databasePath = Path.Join(_keyServer.Configuration.PersistencePath, "store", Configuration.StoreKey);
            Directory.CreateDirectory(databasePath);

            _keyServer.InvokeOnLog(KkErrorLevel.Information, $"Instantiating persistent key-store for [{Configuration.StoreKey}].");
            var options = new DbOptions().SetCreateIfMissing(true);
            var persistenceDatabase = RocksDb.Open(options, databasePath);

            _database = new(persistenceDatabase);
        }

        public void Stop()
        {
            _keyServer.InvokeOnLog(KkErrorLevel.Information, $"Signaling shutdown for [{Configuration.StoreKey}].");

            _database?.Write(o =>
            {
                o.Checkpoint();
                o.Dispose();
            });
        }

        public long CurrentValueCount()
        {
            if (Configuration.PersistenceScheme == KkPersistenceScheme.Persistent)
            {
                return _database?.Read(db =>
                {
                    using var iterator = db.NewIterator();
                    long count = 0;
                    iterator.SeekToFirst();
                    while (iterator.Valid())
                    {
                        count++;
                        iterator.Next();
                    }

                    return count;
                }) ?? 0;
            }
            else if (Configuration.PersistenceScheme == KkPersistenceScheme.Ephemeral)
            {
                return _memoryCache.Count();
            }
            throw new Exception("Undefined PersistenceScheme.");
        }

        #region String.

        public void StringSet(string valueKey, string value)
        {
            if (Configuration.ValueType != KkValueType.String)
            {
                throw new Exception($"StringSet is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;
            _memoryCache.Upsert(valueKey, value, Configuration.CacheExpiration);
            _database?.Write(db => db.Put(valueKey, value));
        }

        public string? StringGet(string valueKey)
        {
            if (Configuration.ValueType != KkValueType.String)
            {
                throw new Exception($"StringGet is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.GetCount++;

            if (_memoryCache.TryGet(valueKey, out string? value) && value != null)
            {
                Statistics.CacheHits++;
                return value;
            }
            Statistics.CacheMisses++;

            return _database?.Read(db =>
            {
                var result = db.Get(valueKey);
                if (result != null)
                {
                    Statistics.DatabaseHits++;
                }
                else
                {
                    Statistics.DatabaseMisses++;
                }

                return result;
            });
        }

        #endregion

        #region List.

        public void ListAdd(string listKey, string valueToAdd)
        {
            if (Configuration.ValueType != KkValueType.StringList)
            {
                throw new Exception($"ListAdd is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;

            _concurrentKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out Dictionary<Guid, string>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                _database?.Read(db =>
                {
                    if (list == null)
                    {
                        //Looks like we did not have the list in memory, so we need to
                        //check the database for the serialized list and deserialize it.
                        var listJson = db.Get(listKey);
                        if (listJson != null)
                        {
                            Statistics.DatabaseHits++;
                            list = JsonSerializer.Deserialize<Dictionary<Guid, string>>(listJson);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    }
                });

                //If the list does not exist. We need to create it.
                list ??= new Dictionary<Guid, string>();

                list.Add(Guid.NewGuid(), valueToAdd);

                //Persist the serialized list.
                _database?.Write(db => db.Put(listKey, JsonSerializer.Serialize(list)));

                //Recache the list.
                _memoryCache.Upsert(listKey, list, Configuration.CacheExpiration);
            });
        }

        public Dictionary<Guid, string>? ListGet(string listKey)
        {
            if (Configuration.ValueType != KkValueType.StringList)
            {
                throw new Exception($"ListGet is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;

            Dictionary<Guid, string>? result = null;

            _concurrentKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out Dictionary<Guid, string>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                _database?.Read(db =>
                {
                    if (list == null)
                    {
                        //Looks like we did not have the list in memory, so we need to
                        //check the database for the serialized list and deserialize it.
                        var listJson = db.Get(listKey);
                        if (listJson != null)
                        {
                            Statistics.DatabaseHits++;
                            list = JsonSerializer.Deserialize<Dictionary<Guid, string>>(listJson);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    }
                });

                result = list?.ToDictionary();
            });

            return result;
        }

        #endregion

        public void Delete(string valueKey)
        {
            Statistics.DeleteCount++;
            _memoryCache.Remove(valueKey);
            _database?.Write(db => db.Remove(valueKey));
        }

        public void Purge()
        {
            _memoryCache.Clear();

            _database?.Write(db =>
            {
                using var iterator = db.NewIterator();
                iterator.SeekToFirst();

                while (iterator.Valid())
                {
                    var key = iterator.Key();
                    db.Remove(key); // Remove the key-value pair
                    iterator.Next();
                }
            });
        }
    }
}
