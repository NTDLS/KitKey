using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStoreCreate(KkStoreConfiguration storeConfiguration)
        : IRmQuery<KkStoreCreateReply>
    {
        public KkStoreConfiguration StoreConfiguration { get; set; } = storeConfiguration;
    }

    public class KkStoreCreateReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStoreCreateReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStoreCreateReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStoreCreateReply()
        {
        }
    }
}
