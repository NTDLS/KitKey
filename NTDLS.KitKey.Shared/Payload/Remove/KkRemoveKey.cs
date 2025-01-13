using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Remove
{
    /// <summary>
    /// API payload used to remove a key from a key-store of any type.
    /// </summary>
    public class KkRemoveKey(string storeKey, string valueKey)
        : IRmQuery<KkRemoveKeyReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ValueKey { get; set; } = valueKey;
    }

    /// <summary>
    /// API payload used to remove a key from a key-store of any type.
    /// </summary>
    public class KkRemoveKeyReply
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
        public KkRemoveKeyReply EnsureSuccessful()
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
        public KkRemoveKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkRemoveKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkRemoveKeyReply()
        {
        }
    }
}
