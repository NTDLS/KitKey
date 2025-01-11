using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfString
{
    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfStringSet(string storeKey, string valueKey, string value)
        : IRmQuery<KkSingleOfStringSetReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ValueKey { get; set; } = valueKey;

        /// <summary>
        /// The value to insert or update to the key-value store for the given ValueKey.
        /// </summary>
        public string Value { get; set; } = value;
    }

    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfStringSetReply
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
        public KkSingleOfStringSetReply EnsureSuccessful()
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
        public KkSingleOfStringSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfStringSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfStringSetReply()
        {
        }
    }
}
