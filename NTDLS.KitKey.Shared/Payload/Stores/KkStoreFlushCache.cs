using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Stores
{
    public class KkStoreFlushCache(string storeKey)
        : IRmQuery<KkStoreFlushCacheReply>
    {
        public string StoreKey { get; set; } = storeKey;
    }

    public class KkStoreFlushCacheReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStoreFlushCacheReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkStoreFlushCacheReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStoreFlushCacheReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStoreFlushCacheReply()
        {
        }
    }
}
