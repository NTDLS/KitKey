using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStringSet(string storeName, string key, string value)
        : IRmQuery<KkStringSetReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public class KkStringSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkStringSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStringSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStringSetReply()
        {
        }
    }
}
