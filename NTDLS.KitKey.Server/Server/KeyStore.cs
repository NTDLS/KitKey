using NTDLS.FastMemoryCache;
using NTDLS.KitKey.Shared;
using NTDLS.Semaphore;
using RocksDbSharp;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// A named key store and its delivery thread.
    /// </summary>
    internal class KeyStore
    {
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

            _database = new(persistenceDatabase);
        }

        public void Stop()
        {
            _keyServer.InvokeOnLog(CMqErrorLevel.Information, $"Signaling shutdown for [{Configuration.StoreName}].");

            _database?.Write(o =>
            {
                o.Checkpoint();
                o.Dispose();
            });
        }

        public void Set(string key, string value)
        {
            _memoryCache.Upsert(key, value);

            _database?.Write(db =>
            {
                Statistics.SetCount++;
                db.Put(key, value);
            });
        }

        public string? Get(string key)
        {
            if (_memoryCache.TryGet(key, out string? value) && value != null)
            {
                return value;
            }

            return _database?.Write(db =>
            {
                Statistics.GetCount++;
                return db.Get(key);
            });
        }

        public void Delete(string key)
        {
            _memoryCache.Remove(key);

            _database?.Write(db =>
            {
                Statistics.DeleteCount++;
                db.Remove(key);
            });
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
