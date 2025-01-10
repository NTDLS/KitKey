using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    public class KkListOfDateTimeGet(string storeKey, string listKey)
        : IRmQuery<KkListOfDateTimeGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDateTimeGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListOfDateTimeGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimeGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimeGetReply()
        {
        }
    }
}
