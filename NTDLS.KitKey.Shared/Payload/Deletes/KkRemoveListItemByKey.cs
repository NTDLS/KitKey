using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Deletes
{
    /// <summary>
    /// Removes a list value from a list-of-values key-store by its id.
    /// </summary>
    public class KkRemoveListItemByKey(string storeKey, string listKey, Guid listItemKey)
        : IRmQuery<KkRemoveListItemByKeyReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Guid ListItemKey { get; set; } = listItemKey;
    }

    public class KkRemoveListItemByKeyReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkRemoveListItemByKeyReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkRemoveListItemByKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkRemoveListItemByKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkRemoveListItemByKeyReply()
        {
        }
    }
}
