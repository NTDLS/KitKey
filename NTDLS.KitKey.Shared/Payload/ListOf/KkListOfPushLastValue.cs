using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf
{
    /// <summary>
    /// API payload used to append a value to a list type key-value store.
    /// </summary>
    public class KkListOfPushLastValue<T>(string storeKey, string listKey, T listValue)
        : IRmQuery<KkListOfPushLastValueReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ListKey { get; set; } = listKey;

        /// <summary>
        /// The value add to the list inside the key-value store for the given ListKey.
        /// </summary>
        public T ListValue { get; set; } = listValue;
    }

    /// <summary>
    /// API payload used to append a value to a list type key-value store.
    /// </summary>
    public class KkListOfPushLastValueReply
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
        public KkListOfPushLastValueReply EnsureSuccessful()
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
        public KkListOfPushLastValueReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfPushLastValueReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfPushLastValueReply()
        {
        }
    }
}
