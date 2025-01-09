using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkSetString(string storeName, string key, string value)
        : IRmQuery<KkSetStringReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public class KkSetStringReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSetStringReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSetStringReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSetStringReply()
        {
        }
    }
}
