using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server.Management
{
    /// <summary>
    /// Defines a queue configuration.
    /// </summary>
    public class KkStoreDescriptor()
    {
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string StoreName { get; internal set; } = string.Empty;

        /// <summary>
        /// The current number of messages that are enqueued in this message queue.
        /// </summary>
        public int CurrentMessageCount { get; internal set; }

        /// <summary>
        /// The total number of messages that have been enqueued into this queue.
        /// </summary>
        public ulong ReceivedMessageCount { get; internal set; }

        /// <summary>
        /// The total number of messages that have been delivered from this queue to subscribers.
        /// </summary>
        public ulong DeliveredMessageCount { get; internal set; }

        /// <summary>
        /// Whether the queue is persisted or ephemeral.
        /// </summary>
        public CMqPersistenceScheme PersistenceScheme { get; set; } = CMqPersistenceScheme.Ephemeral;
    }
}
