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
        public string StoreName { get; internal set; } = string.Empty;

        /// <summary>
        /// Whether the key-store is persisted or ephemeral.
        /// </summary>
        public CMqPersistenceScheme PersistenceScheme { get; set; } = CMqPersistenceScheme.Ephemeral;

        /// <summary>
        /// The current number of values in this key store.
        /// </summary>
        public int CurrentValueCount { get; internal set; }

        /// <summary>
        /// The count of values that have been inserted/updated into the key-store
        /// </summary>
        public ulong SetCount { get; set; }

        /// <summary>
        /// The count of values that have been removed into the key-store
        /// </summary>
        public ulong DeleteCount { get; set; }

        /// <summary>
        /// The count of values that have been retrieved from the key-store
        /// </summary>
        public ulong GetCount { get; set; }

        /// <summary>
        /// The count of gets that were satisfied by the key-store cache.
        /// </summary>
        public ulong CacheHits { get; set; }

        /// <summary>
        /// The count of gets that were satisfied by the key-store database.
        /// </summary>
        public ulong DatabaseHits { get; set; }

        /// <summary>
        /// The count of gets that were not satisfied by the key-store cache.
        /// </summary>
        public ulong CacheMisses { get; set; }

        /// <summary>
        /// The count of gets that were not satisfied by the key-store database.
        /// </summary>
        public ulong DatabaseMisses { get; set; }
    }
}
