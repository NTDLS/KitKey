using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkListGet(string storeKey, string listKey)
        : IRmQuery<KkListGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListGetReply()
        {
        }
    }
}
