using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Stores
{
    /// <summary>
    /// API payload used to flush the memory cache of all key-value stores.
    /// </summary>
    public class KkStoreFlushAllCaches()
        : IRmQuery<KkStoreFlushAllCachesReply>
    {
    }

    /// <summary>
    /// API payload used to flush the memory cache of all key-value stores.
    /// </summary>

    public class KkStoreFlushAllCachesReply
        : IRmQueryReply
    {
        /// <summary>
        /// Indicated whether the original query was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary> 
        /// When IsSuccess is not true, ErrorMessage indicates the error which occurred. 
        /// </summary> 
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Throws the ErrorMessage where IsSuccess is not true.
        /// </summary>
        public KkStoreFlushAllCachesReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkStoreFlushAllCachesReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkStoreFlushAllCachesReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkStoreFlushAllCachesReply()
        {
        }
    }
}
