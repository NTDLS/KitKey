using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    public class KkListOfDoubleGet(string storeKey, string listKey)
        : IRmQuery<KkListOfDoubleGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDoubleGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListOfDoubleGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoubleGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoubleGetReply()
        {
        }
    }
}
