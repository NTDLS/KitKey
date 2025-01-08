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
    }
}
