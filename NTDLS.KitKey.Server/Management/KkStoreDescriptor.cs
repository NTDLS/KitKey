using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server.Management
{
    /// <summary>
    /// Defines a key-store configuration.
    /// </summary>
    public class KkStoreDescriptor()
    {
        /// <summary>
        /// The name of the key-store.
        /// </summary>
        public string StoreKey { get; internal set; } = string.Empty;

        /// <summary>
        /// Whether the key-store is persisted or ephemeral.
        /// </summary>
        public KkPersistenceScheme PersistenceScheme { get; set; } = KkPersistenceScheme.Ephemeral;

        /// <summary>
        /// Specifies the type of values or values that the store will contain.
        /// </summary>
        public KkValueType ValueType { get; set; } = KkValueType.String;

        /// <summary>
        /// The amount of (sliding expiration) time that a key/value should stay in cache. If not defined, persistent
        /// key-stores will use KkDefaults.DEFAULT_CACHE_SECONDS while ephemeral key-stores will be 1 day (since they
        /// live solely in cache). Note that defining an expiration for ephemeral key-stores will result in loss of the
        /// values once they expire.
        /// </summary>
        public TimeSpan? CacheExpiration { get; set; }

        /// <summary>
        /// The current number of values in this key store.
        /// </summary>
        public long CurrentValueCount { get; internal set; }

        /// <summary>
        /// The count of values that have been inserted/updated into the key-store
        /// </summary>
        public long SetCount { get; set; }

        /// <summary>
        /// The count of values that have been removed into the key-store
        /// </summary>
        public long DeleteCount { get; set; }

        /// <summary>
        /// The count of values that have been retrieved from the key-store
        /// </summary>
        public long GetCount { get; set; }

        /// <summary>
        /// The count of gets that were satisfied by the key-store cache.
        /// </summary>
        public long CacheHits { get; set; }

        /// <summary>
        /// The count of gets that were satisfied by the key-store database.
        /// </summary>
        public long DatabaseHits { get; set; }

        /// <summary>
        /// The count of gets that were not satisfied by the key-store cache.
        /// </summary>
        public long CacheMisses { get; set; }

        /// <summary>
        /// The count of gets that were not satisfied by the key-store database.
        /// </summary>
        public long DatabaseMisses { get; set; }
    }
}
