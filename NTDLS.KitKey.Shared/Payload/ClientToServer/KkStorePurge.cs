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

        public KkStorePurgeReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

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
