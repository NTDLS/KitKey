using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Stores
{
    public class KkStoreFlushAllCaches()
        : IRmQuery<KkStoreFlushAllCachesReply>
    {
    }

    public class KkStoreFlushAllCachesReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStoreFlushAllCachesReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkStoreFlushAllCachesReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStoreFlushAllCachesReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStoreFlushAllCachesReply()
        {
        }
    }
}
