using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkCreateStore(KkStoreConfiguration storeConfiguration)
        : IRmQuery<KkCreateStoreReply>
    {
        public KkStoreConfiguration StoreConfiguration { get; set; } = storeConfiguration;
    }

    public class KkCreateStoreReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkCreateStoreReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkCreateStoreReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkCreateStoreReply()
        {
        }
    }
}
