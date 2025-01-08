﻿using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server
{
    /// <summary>
    /// Key-store client configuration parameters.
    /// </summary>
    public class KkServerConfiguration
    {
        /// <summary>
        /// When true, query replies are queued in a thread pool. Otherwise, queries block other activities.
        /// </summary>
        public bool AsynchronousAcknowledgment { get; set; } = true;

        /// <summary>
        /// The default amount of time to wait for a query to reply before throwing a timeout exception.
        /// </summary>
        public int AcknowledgmentTimeoutSeconds { get; set; } = KkDefaults.DEFAULT_ACK_TIMEOUT_SECONDS;
        /// <summary>
        /// The initial size in bytes of the receive buffer.
        /// If the buffer ever gets full while receiving data it will be automatically resized up to MaxReceiveBufferSize.
        /// </summary>
        public int InitialReceiveBufferSize { get; set; } = KkDefaults.DEFAULT_INITIAL_BUFFER_SIZE;

        /// <summary>
        ///The maximum size in bytes of the receive buffer.
        ///If the buffer ever gets full while receiving data it will be automatically resized up to MaxReceiveBufferSize.
        /// </summary>
        public int MaxReceiveBufferSize { get; set; } = KkDefaults.DEFAULT_MAX_BUFFER_SIZE;

        /// <summary>
        ///The growth rate of the auto-resizing for the receive buffer.
        /// </summary>
        public double ReceiveBufferGrowthRate { get; set; } = KkDefaults.DEFAULT_BUFFER_GROWTH_RATE;

        /// <summary>
        /// For persistent key-stores, this is where the data will be stored.
        /// </summary>
        public string? PersistencePath { get; set; }
    }
}