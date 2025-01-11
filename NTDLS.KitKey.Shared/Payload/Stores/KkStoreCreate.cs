using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Stores
{
    /// <summary>
    /// API payload used to create a key-value store.
    /// </summary>
    public class KkStoreCreate(KkStoreConfiguration storeConfiguration)
        : IRmQuery<KkStoreCreateReply>
    {
        /// <summary>
        /// Denotes the configuration for the new key-value store.
        /// </summary>
        public KkStoreConfiguration StoreConfiguration { get; set; } = storeConfiguration;
    }

    /// <summary>
    /// API payload used to create a key-value store.
    /// </summary>

    public class KkStoreCreateReply
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
        public KkStoreCreateReply EnsureSuccessful()
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
        public KkStoreCreateReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkStoreCreateReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkStoreCreateReply()
        {
        }
    }
}
