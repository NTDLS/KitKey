namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// Statistics for a single key store.
    /// </summary>
    internal class KeyStoreStatistics
    {
        /// <summary>
        /// The total number of values that have been sent into this key-store.
        /// </summary>
        public ulong UpsertCount { get; set; }

        /// <summary>
        /// The total number of values that have been retrieved from this key-store to subscribers.
        /// </summary>
        public ulong GetCount { get; set; }
    }
}
