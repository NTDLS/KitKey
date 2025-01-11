using NTDLS.FastMemoryCache;
using NTDLS.KitKey.Shared;
using NTDLS.Semaphore;
using RocksDbSharp;
using System.Text;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// A named key-store, its cache and persistence database.
    /// </summary>
    internal class KeyStore
    {
        private readonly AtomicKeyOperation _atomicKeyOperation = new();
        private readonly KkClient _keyServer;
        private OptimisticCriticalResource<RocksDb>? _database;
        private readonly PartitionedMemoryCache _memoryCache;

        internal KkStoreConfiguration Configuration { get; private set; }
        internal KeyStoreStatistics Statistics { get; set; } = new();

        public KeyStore(KkClient keyServer, KkStoreConfiguration storeConfiguration)
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

        public void FlushCache()
        {
            _keyServer.InvokeOnLog(KkErrorLevel.Information, $"Clearing the cache for [{Configuration.StoreKey}].");
            _memoryCache.Clear();
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

        private Type EnsureProperType<T>(bool isList)
        {
            var typeOf = typeof(T);

            #region Type Validation.

            if ((isList && Configuration.ValueType == KkValueType.ListOfStrings) || (!isList && Configuration.ValueType == KkValueType.String))
            {
                if (typeOf != typeof(string))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfGuids) || (!isList && Configuration.ValueType == KkValueType.Guid))
            {
                if (typeOf != typeof(Guid))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfInt32s) || (!isList && Configuration.ValueType == KkValueType.Int32))
            {
                if (typeOf != typeof(Int32))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfInt64s) || (!isList && Configuration.ValueType == KkValueType.Int64))
            {
                if (typeOf != typeof(Int64))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfSingles) || (!isList && Configuration.ValueType == KkValueType.Single))
            {
                if (typeOf != typeof(Single))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfDoubles) || (!isList && Configuration.ValueType == KkValueType.Double))
            {
                if (typeOf != typeof(Double))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else if ((isList && Configuration.ValueType == KkValueType.ListOfDateTimes) || (!isList && Configuration.ValueType == KkValueType.DateTime))
            {
                if (typeOf != typeof(DateTime))
                    throw new Exception($"Key-store [{Configuration.StoreKey}] can only contain list values of type: [{Configuration.ValueType}].");
            }
            else
            {
                throw new Exception($"Invalid value type for key-store [{Configuration.StoreKey}], expected: [{Configuration.ValueType}], found: [{typeOf.Name}].");
            }

            #endregion

            return typeOf;
        }

        private byte[] GenericToBytes<T>(T value, bool isList)
        {
            EnsureProperType<T>(isList);

            if (value is string stringValue)
                return Encoding.UTF8.GetBytes(stringValue);
            else if (value is Guid guidValue)
                return guidValue.ToByteArray();
            else if (value is Int32 int32Value)
                return BitConverter.GetBytes(int32Value);
            else if (value is Int64 int64Value)
                return BitConverter.GetBytes(int64Value);
            else if (value is Single singleValue)
                return BitConverter.GetBytes(singleValue);
            else if (value is Double doubleValue)
                return BitConverter.GetBytes(doubleValue);
            else if (value is DateTime dateTimeValue)
                return BitConverter.GetBytes(dateTimeValue.Ticks);

            throw new Exception($"Invalid value type for key-store [{Configuration.StoreKey}], expected: [{Configuration.ValueType}], found: [{typeof(T).Name}].");
        }

        private T GenericFromBytes<T>(byte[] bytes, bool isList)
        {
            var typeOf = EnsureProperType<T>(isList);

            if (typeOf == typeof(string))
                return (T)(object)Encoding.UTF8.GetString(bytes);
            else if (typeOf == typeof(Guid))
                return (T)(object)new Guid(bytes);
            else if (typeOf == typeof(Int32))
                return (T)(object)BitConverter.ToInt32(bytes);
            else if (typeOf == typeof(Int64))
                return (T)(object)BitConverter.ToInt64(bytes);
            else if (typeOf == typeof(Single))
                return (T)(object)BitConverter.ToSingle(bytes);
            else if (typeOf == typeof(Double))
                return (T)(object)BitConverter.ToDouble(bytes);
            else if (typeOf == typeof(DateTime))
                return (T)(object)(new DateTime(BitConverter.ToInt64(bytes), DateTimeKind.Utc));

            throw new Exception($"Invalid value type for key-store [{Configuration.StoreKey}], expected: [{Configuration.ValueType}], found: [{typeOf.Name}].");
        }

        #endregion

        public void SetSingleValue<T>(string valueKey, T value)
        {
            Statistics.SetCount++;

            if (value == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            EnsureProperType<T>(false);

            var valueBytes = GenericToBytes<T>(value, false);
            _memoryCache.Upsert(valueKey, value, Configuration.CacheExpiration);
            var valueKeyBytes = Encoding.UTF8.GetBytes(valueKey);
            _database?.Write(db => db.Put(valueKeyBytes, valueKeyBytes.Length, valueBytes, valueBytes.Length));
        }

        public T? GetSingleValue<T>(string valueKey)
        {
            Statistics.GetCount++;

            EnsureProperType<T>(false);

            var valueKeyBytes = Encoding.UTF8.GetBytes(valueKey);

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
                        return GenericFromBytes<T>(resultBytes, false);
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

        #region List of Values.


        public void DeleteListItemByKey(string listKey, Guid listItemKey)
        {
            Statistics.DeleteCount++;

            var listKeyBytes = Encoding.UTF8.GetBytes(listKey);

            _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                //If the list does not exist. We need to create it.
                list ??= new List<BinaryListItem>();

                //Remove the item from the list.
                list.RemoveAll(o => o.Id == listItemKey);

                //Persist the serialized list.
                var valueListBytes = BinarySerialization.ToBytes(list);
                _database?.Write(db => db.Put(listKeyBytes, listKeyBytes.Length, valueListBytes, valueListBytes.Length));

                //Cache the list.
                _memoryCache.Upsert(listKey, list, Configuration.CacheExpiration);
            });
        }

        public void PushLast<T>(string listKey, T valueToAdd)
        {
            Statistics.SetCount++;

            if (valueToAdd == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            EnsureProperType<T>(true);

            var listKeyBytes = Encoding.UTF8.GetBytes(listKey);

            _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                //If the list does not exist. We need to create it.
                list ??= new List<BinaryListItem>();

                //Add item to the list.
                var valueBytes = GenericToBytes<T>(valueToAdd, true);
                list.Add(new BinaryListItem(valueBytes));

                //Persist the serialized list.
                var valueListBytes = BinarySerialization.ToBytes(list);
                _database?.Write(db => db.Put(listKeyBytes, listKeyBytes.Length, valueListBytes, valueListBytes.Length));

                //Cache the list.
                _memoryCache.Upsert(listKey, list, Configuration.CacheExpiration);
            });
        }

        public void PushFirst<T>(string listKey, T valueToAdd)
        {
            Statistics.SetCount++;

            if (valueToAdd == null)
            {
                throw new Exception("Key-value stores do not allow null values.");
            }

            EnsureProperType<T>(true);

            var listKeyBytes = Encoding.UTF8.GetBytes(listKey);

            _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                //If the list does not exist. We need to create it.
                list ??= new List<BinaryListItem>();

                //Add item to the list.
                var valueBytes = GenericToBytes<T>(valueToAdd, true);
                list.Insert(0, new BinaryListItem(valueBytes));

                //Persist the serialized list.
                var valueListBytes = BinarySerialization.ToBytes(list);
                _database?.Write(db => db.Put(listKeyBytes, listKeyBytes.Length, valueListBytes, valueListBytes.Length));

                //Cache the list.
                _memoryCache.Upsert(listKey, list, Configuration.CacheExpiration);
            });
        }

        public List<KkListItem<T>>? GetList<T>(string listKey)
        {
            Statistics.SetCount++;

            EnsureProperType<T>(true);

            return _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var listKeyBytes = Encoding.UTF8.GetBytes(listKey);
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                return list?.Select(o => new KkListItem<T>(o.Id, GenericFromBytes<T>(o.Bytes, true))).ToList();
            });
        }

        public KkListItem<T>? GetFirst<T>(string listKey)
        {
            Statistics.SetCount++;

            EnsureProperType<T>(true);

            return _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var listKeyBytes = Encoding.UTF8.GetBytes(listKey);
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                var firstListItem = list?.FirstOrDefault();
                if (firstListItem != null)
                {
                    return new KkListItem<T>(firstListItem.Id, GenericFromBytes<T>(firstListItem.Bytes, true));
                }

                return default;
            });
        }

        public KkListItem<T>? GetLast<T>(string listKey)
        {
            Statistics.SetCount++;

            EnsureProperType<T>(true);

            return _atomicKeyOperation.Execute(listKey, () =>
            {
                //See if we have the list in memory.
                if (_memoryCache.TryGet(listKey, out List<BinaryListItem>? list) && list != null)
                {
                    Statistics.CacheHits++;
                }
                else
                {
                    Statistics.CacheMisses++;
                }

                if (list == null) //didn't get a value from cache, so let's try the database.
                {
                    _database?.Read(db =>
                    {
                        var listKeyBytes = Encoding.UTF8.GetBytes(listKey);
                        var valueListBytes = db.Get(listKeyBytes, listKeyBytes.Length);
                        if (valueListBytes != null)
                        {
                            Statistics.DatabaseHits++;
                            list = BinarySerialization.FromBytes<List<BinaryListItem>>(valueListBytes);
                        }
                        else
                        {
                            Statistics.DatabaseHits++;
                        }
                    });
                }

                var lastListItem = list?.LastOrDefault();
                if (lastListItem != null)
                {
                    return new KkListItem<T>(lastListItem.Id, GenericFromBytes<T>(lastListItem.Bytes, true));
                }

                return default;
            });
        }

        #endregion

        public void DeleteKey(string valueKey)
        {
            Statistics.DeleteCount++;
            _memoryCache.Remove(valueKey);
            _database?.Write(db => db.Remove(valueKey));
        }

        public void StorePurge()
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
