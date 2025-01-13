using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf
{
    /// <summary>
    /// API payload used to get all items from a list type key-value store.
    /// </summary>
    public class KkListOfGetAll<T>(string storeKey, string listKey)
        : IRmQuery<KkListOfGetAllReply<T>>
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
    /// API payload used to get all items from a list type key-value store.
    /// </summary>
    public class KkListOfGetAllReply<T>
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
        /// All items stored in the key-value store for the given ListKey.
        /// </summary>
        public List<KkListItem<T>>? List { get; set; }

        /// <summary>
        /// Throws the ErrorMessage where IsSuccess is not true.
        /// </summary>
        public KkListOfGetAllReply<T> EnsureSuccessful()
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
        public KkListOfGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfGetAllReply()
        {
        }
    }
}
