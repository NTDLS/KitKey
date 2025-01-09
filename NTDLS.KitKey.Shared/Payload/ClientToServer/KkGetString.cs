using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkGetString(string storeName, string key)
        : IRmQuery<KkGetStringReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
    }

    public class KkGetStringReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Value { get; set; }

        public KkGetStringReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkGetStringReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkGetStringReply()
        {
        }
    }
}
