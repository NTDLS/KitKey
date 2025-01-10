using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    public class KkLisOfDoubleGet(string storeKey, string listKey)
        : IRmQuery<KkLisOfDoubleGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkLisOfDoubleGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkLisOfDoubleGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkLisOfDoubleGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkLisOfDoubleGetReply()
        {
        }
    }
}
