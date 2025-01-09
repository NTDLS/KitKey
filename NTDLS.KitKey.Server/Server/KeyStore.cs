﻿using NTDLS.FastMemoryCache;
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

            _keyServer.InvokeOnLog(KkErrorLevel.Verbose, $"Creating persistent path for [{Configuration.StoreName}].");

            var databasePath = Path.Join(_keyServer.Configuration.PersistencePath, "store", Configuration.StoreName);
            Directory.CreateDirectory(databasePath);

            _keyServer.InvokeOnLog(KkErrorLevel.Information, $"Instantiating persistent key-store for [{Configuration.StoreName}].");
            var options = new DbOptions().SetCreateIfMissing(true);
            var persistenceDatabase = RocksDb.Open(options, databasePath);

            _database = new(persistenceDatabase);
        }

        public void Stop()
        {
            _keyServer.InvokeOnLog(KkErrorLevel.Information, $"Signaling shutdown for [{Configuration.StoreName}].");

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

        public void SetString(string key, string value)
        {
            if (Configuration.ValueType != KkValueType.String)
            {
                throw new Exception($"SetString is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;
            _memoryCache.Upsert(key, value, Configuration.CacheExpiration);
            _database?.Write(db => db.Put(key, value));
        }

        public string? GetString(string key)
        {
            if (Configuration.ValueType != KkValueType.String)
            {
                throw new Exception($"GetString is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.GetCount++;

            if (_memoryCache.TryGet(key, out string? value) && value != null)
            {
                Statistics.CacheHits++;
                return value;
            }
            Statistics.CacheMisses++;

            return _database?.Read(db =>
            {
                var result = db.Get(key);
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

        public void AppendList(string key, string valueToAdd)
        {
            if (Configuration.ValueType != KkValueType.StringList)
            {
                throw new Exception($"AppendList is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;

            _concurrentKeyOperation.Execute(key, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(key, out List<KkListItem>? list) && list != null)
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
                        var listJson = db.Get(key);
                        if (listJson != null)
                        {
                            Statistics.DatabaseHits++;
                            list = JsonSerializer.Deserialize<List<KkListItem>>(listJson);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    }
                });

                //If the list does not exist. We need to create it.
                list ??= new List<KkListItem>();

                list.Add(new KkListItem()
                {
                    Id = Guid.NewGuid(),
                    Value = valueToAdd
                });

                //Persist the serialized list.
                _database?.Write(db => db.Put(key, JsonSerializer.Serialize(list)));

                //Recache the list.
                _memoryCache.Upsert(key, list, Configuration.CacheExpiration);
            });
        }

        public List<KkListItem>? GetList(string key)
        {
            if (Configuration.ValueType != KkValueType.StringList)
            {
                throw new Exception($"GetList is invalid for the key-store type: [{Configuration.ValueType}].");
            }

            Statistics.SetCount++;

            List<KkListItem>? result = null;

            _concurrentKeyOperation.Execute(key, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(key, out List<KkListItem>? list) && list != null)
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
                        var listJson = db.Get(key);
                        if (listJson != null)
                        {
                            Statistics.DatabaseHits++;
                            list = JsonSerializer.Deserialize<List<KkListItem>>(listJson);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    }
                });

                result = list?.ToList();
            });

            return result;
        }

        #endregion

        public void Delete(string key)
        {
            Statistics.DeleteCount++;
            _memoryCache.Remove(key);
            _database?.Write(db => db.Remove(key));
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
