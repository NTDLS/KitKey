using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStringGet(string storeName, string key)
        : IRmQuery<KkStringGetReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
    }

    public class KkStringGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Value { get; set; }

        public KkStringGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkStringGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkStringGetReply()
        {
        }
    }
}
