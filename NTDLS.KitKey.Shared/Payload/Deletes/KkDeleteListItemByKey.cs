using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Deletes
{
    /// <summary>
    /// Removes a list value from a list-of-values key-store by its id.
    /// </summary>
    public class KkDeleteListItemByKey(string storeKey, string listKey, Guid listItemKey)
        : IRmQuery<KkDeleteListItemByKeyReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Guid ListItemKey { get; set; } = listItemKey;
    }

    public class KkDeleteListItemByKeyReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkDeleteListItemByKeyReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkDeleteListItemByKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkDeleteListItemByKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkDeleteListItemByKeyReply()
        {
        }
    }
}
