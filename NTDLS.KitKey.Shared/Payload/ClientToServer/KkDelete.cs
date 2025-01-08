using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkDelete(string storeName, string key)
        : IRmQuery<KkDeleteReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
    }

    public class KkDeleteReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkDeleteReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkDeleteReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkDeleteReply()
        {
        }
    }
}
