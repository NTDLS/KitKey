using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Deletes
{
    /// <summary>
    /// API payload used to remove a list value from a list-of-values key-store by its id.
    /// </summary>
    public class KkRemoveListItemByKey(string storeKey, string listKey, Guid listItemKey)
        : IRmQuery<KkRemoveListItemByKeyReply>
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
        /// The key (or identifier) of the list item value in the key-value store list.
        /// </summary>
        public Guid ListItemKey { get; set; } = listItemKey;
    }

    /// <summary>
    /// API payload used to remove a list value from a list-of-values key-store by its id.
    /// </summary>
    public class KkRemoveListItemByKeyReply
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
        public KkRemoveListItemByKeyReply EnsureSuccessful()
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
        public KkRemoveListItemByKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkRemoveListItemByKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkRemoveListItemByKeyReply()
        {
        }
    }
}
