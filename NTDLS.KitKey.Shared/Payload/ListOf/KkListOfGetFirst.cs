using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf
{
    /// <summary>
    /// API payload used to get the first item from a list type key-value store.
    /// </summary>
    public class KkListOfGetFirst<T>(string storeKey, string listKey)
        : IRmQuery<KkListOfGetFirstReply<T>>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ListKey { get; set; } = listKey;
    }

    /// <summary>
    /// API payload used to get the first item from a list type key-value store.
    /// </summary>
    public class KkListOfGetFirstReply<T>
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
        /// The value retrieved from the list inside the key-value store for the given ListKey.
        /// </summary>
        public KkListItem<T>? Value { get; set; }

        /// <summary>
        /// Throws the ErrorMessage where IsSuccess is not true.
        /// </summary>
        public KkListOfGetFirstReply<T> EnsureSuccessful()
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
        public KkListOfGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfGetFirstReply()
        {
        }
    }
}
