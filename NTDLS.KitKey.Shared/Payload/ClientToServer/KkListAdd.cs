using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListAdd(string storeKey, string listKey, string listValue)
        : IRmQuery<KkListAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public string ListValue { get; set; } = listValue;
    }

    public class KkListAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListAddReply()
        {
        }
    }
}
