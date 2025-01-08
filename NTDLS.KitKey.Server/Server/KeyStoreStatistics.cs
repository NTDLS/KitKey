namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// Statistics for a single key store.
    /// </summary>
    internal class KeyStoreStatistics
    {
        /// <summary>
        /// The count of values stored in the key-store.
        /// </summary>
        public long ValueCount { get; set; }

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
