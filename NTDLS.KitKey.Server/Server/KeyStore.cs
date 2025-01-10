using NTDLS.FastMemoryCache;
using NTDLS.KitKey.Shared;
using NTDLS.Semaphore;
using RocksDbSharp;
using System.Text;
using System.Text.Json;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// A named key-store, its cache and persistence database.
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

        #region Generic Conversion.

        private byte[] GenericToBytes<T>(T value) where T : notnull
        {
            if (value is string stringValue)
            {
                if (Configuration.ValueType != KkValueType.String)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return Encoding.UTF8.GetBytes(stringValue);
            }
            else if (value is int int32Value)
            {
                if (Configuration.ValueType != KkValueType.Int32)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return BitConverter.GetBytes(int32Value);
            }
            else if (value is Int64 int64Value)
            {
                if (Configuration.ValueType != KkValueType.Int64)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return BitConverter.GetBytes(int64Value);
            }
            else if (value is float floatValue)
            {
                if (Configuration.ValueType != KkValueType.Float)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return BitConverter.GetBytes(floatValue);
            }
            else if (value is double doubleValue)
            {
                if (Configuration.ValueType != KkValueType.Double)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return BitConverter.GetBytes(doubleValue);
            }
            else if (value is DateTime dateTimeValue)
            {
                if (Configuration.ValueType != KkValueType.DateTime)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return BitConverter.GetBytes(dateTimeValue.Ticks);
            }

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        private T? GenericFromBytes<T>(byte[] bytes)
        {
            var genericType = typeof(T);

            if (genericType == typeof(string))
            {
                if (Configuration.ValueType != KkValueType.String)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return (T)(object)Encoding.UTF8.GetString(bytes);
            }
            else if (genericType == typeof(int))
            {
                if (Configuration.ValueType != KkValueType.Int32)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return (T)(object)BitConverter.ToInt32(bytes);
            }
            else if (genericType == typeof(Int64))
            {
                if (Configuration.ValueType != KkValueType.Int64)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return (T)(object)BitConverter.ToInt64(bytes);
            }
            else if (genericType == typeof(float))
            {
                if (Configuration.ValueType != KkValueType.Float)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return (T)(object)BitConverter.ToSingle(bytes);
            }
            else if (genericType == typeof(double))
            {
                if (Configuration.ValueType != KkValueType.Double)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                return (T)(object)BitConverter.ToDouble(bytes);
            }
            else if (genericType == typeof(DateTime))
            {
                if (Configuration.ValueType != KkValueType.DateTime)
                {
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain values of type: [{Configuration.ValueType}].");
                }
                long ticks = BitConverter.ToInt64(bytes);
                return (T)(object)(new DateTime(ticks, DateTimeKind.Utc));
            }

            throw new Exception($"Key-store [{typeof(T).Name}] is not implemented.");
        }

        #endregion

        public void SetValue<T>(string valueKey, T value) where T : notnull
        {
            var valueBytes = GenericToBytes<T>(value);

            Statistics.SetCount++;
            _memoryCache.Upsert(valueKey, value, Configuration.CacheExpiration);
            var valueKeyBytes = Encoding.UTF8.GetBytes(valueKey);
            _database?.Write(db => db.Put(valueKeyBytes, valueKeyBytes.Length, valueBytes, valueBytes.Length));
        }

        public T? GetValue<T>(string valueKey)
        {
            var valueKeyBytes = Encoding.UTF8.GetBytes(valueKey);

            Statistics.GetCount++;

            if (_memoryCache.TryGet(valueKey, out T? value) && value != null)
            {
                Statistics.CacheHits++;
                return value;
            }
            Statistics.CacheMisses++;

            if (_database != null)
            {
                return _database.Read(db =>
                {
                    var resultBytes = db.Get(valueKeyBytes, valueKeyBytes.Length);
                    if (resultBytes != null)
                    {
                        Statistics.DatabaseHits++;
                        return GenericFromBytes<T>(resultBytes);
                    }
                    else
                    {
                        Statistics.DatabaseMisses++;
                        return default;
                    }

                });
            }

            return default;
        }

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
