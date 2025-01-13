namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// Defines default configuration values for KitKey.
    /// </summary>
    public static class KkDefaults
    {
        /// <summary>
        /// The amount of (sliding expiration) time that a key/value should stay in cache. 
        /// </summary>
        public const int DEFAULT_CACHE_SECONDS = 60;

        /// <summary>
        /// The initial size in bytes of the buffer. If the buffer ever gets full while receiving
        /// data it will be automatically resized by a factor of BUFFER_GROWTH_RATE up to MAX_BUFFER_SIZE.
        /// </summary>
        public const int DEFAULT_INITIAL_BUFFER_SIZE = 16 * 1024;

        /// <summary>
        /// The maximum size in bytes of the buffer. If the buffer ever gets full while receiving
        /// data it will be automatically resized by a factor of BUFFER_GROWTH_RATE up to MAX_BUFFER_SIZE.
        /// </summary>
        public const int DEFAULT_MAX_BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        ///The growth rate for auto-resizing the buffer. Expressed in decimal percentages.
        /// </summary>
        public const double DEFAULT_BUFFER_GROWTH_RATE = 0.2;

        /// <summary>
        /// The default amount of time to wait for a query to reply before throwing a timeout exception.
        /// </summary>
        public const int DEFAULT_ACK_TIMEOUT_SECONDS = 30;

        /// <summary>
        /// The port which the key-store server and client communicate on.
        /// </summary>
        public const int DEFAULT_KEYSTORE_PORT = 45488;
    }
}
