using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStoreDelete(string storeName)
        : IRmQuery<KkStoreDeleteReply>
    {
        public string StoreName { get; set; } = storeName;
    }

    public class KkStoreDeleteReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStoreDeleteReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStoreDeleteReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStoreDeleteReply()
        {
        }
    }
}
