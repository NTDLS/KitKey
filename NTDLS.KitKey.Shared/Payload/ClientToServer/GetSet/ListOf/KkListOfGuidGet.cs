using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    public class KkListOfGuidGet(string storeKey, string listKey)
        : IRmQuery<KkListOfGuidGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfGuidGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, Guid>? List { get; set; }

        public KkListOfGuidGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidGetReply()
        {
        }
    }
}
