using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkPurgeStore(string storeName)
        : IRmQuery<KkPurgeStoreReply>
    {
        public string StoreName { get; set; } = storeName;
    }

    public class KkPurgeStoreReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkPurgeStoreReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkPurgeStoreReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkPurgeStoreReply()
        {
        }
    }
}
