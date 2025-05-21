using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server
{
    /// <summary>
    /// Key-store client configuration parameters.
    /// </summary>
    public class KkServerDescriptor
    {
        /// <summary>
        /// The default amount of time to wait for a query to reply before throwing a timeout exception.
        /// </summary>
        public int AcknowledgmentTimeoutSeconds { get; internal set; } = KkDefaults.DEFAULT_ACK_TIMEOUT_SECONDS;

        /// <summary>
        /// The initial size in bytes of the receive buffer.
        /// If the buffer ever gets full while receiving data it will be automatically resized up to MaxReceiveBufferSize.
        /// </summary>
        public int InitialReceiveBufferSize { get; internal set; } = KkDefaults.DEFAULT_INITIAL_BUFFER_SIZE;

        /// <summary>
        ///The maximum size in bytes of the receive buffer.
        ///If the buffer ever gets full while receiving data it will be automatically resized up to MaxReceiveBufferSize.
        /// </summary>
        public int MaxReceiveBufferSize { get; internal set; } = KkDefaults.DEFAULT_MAX_BUFFER_SIZE;

        /// <summary>
        ///The growth rate of the auto-resizing for the receive buffer.
        /// </summary>
        public double ReceiveBufferGrowthRate { get; internal set; } = KkDefaults.DEFAULT_BUFFER_GROWTH_RATE;

        /// <summary>
        /// The TCP/IP port that the key store server is listening on.
        /// </summary>
        public int ListenPort { get; internal set; }
        /// <summary>
        /// For persistent queues, this is where the data will be stored.
        /// </summary>
        public string? PersistencePath { get; internal set; }
    }
}
