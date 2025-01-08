namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// Statistics for a single key store.
    /// </summary>
    internal class KeyStoreStatistics
    {
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
