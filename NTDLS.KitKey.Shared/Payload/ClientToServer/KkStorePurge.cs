using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStorePurge(string storeKey)
        : IRmQuery<KkStorePurgeReply>
    {
        public string StoreKey { get; set; } = storeKey;
    }

    public class KkStorePurgeReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStorePurgeReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStorePurgeReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStorePurgeReply()
        {
        }
    }
}
