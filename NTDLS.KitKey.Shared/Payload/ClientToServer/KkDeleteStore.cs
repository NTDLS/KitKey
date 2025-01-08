using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkDeleteStore(string storeName)
        : IRmQuery<KkDeleteStoreReply>
    {
        public string StoreName { get; set; } = storeName;
    }

    public class KkDeleteStoreReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkDeleteStoreReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkDeleteStoreReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkDeleteStoreReply()
        {
        }
    }
}
