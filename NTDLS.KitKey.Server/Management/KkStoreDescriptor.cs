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
        /// The current number of values in this key store.
        /// </summary>
        public int CurrentValueCount { get; internal set; }

        /// <summary>
        /// The total number of values that have been sent into this key-store.
        /// </summary>
        public ulong UpsertCount { get; internal set; }

        /// <summary>
        /// The total number of values that have been retrieved from this key-store to subscribers.
        /// </summary>
        public ulong GetCount { get; internal set; }

        /// <summary>
        /// Whether the key-store is persisted or ephemeral.
        /// </summary>
        public CMqPersistenceScheme PersistenceScheme { get; set; } = CMqPersistenceScheme.Ephemeral;
    }
}
