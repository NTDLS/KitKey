using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkListGet(string storeName, string key)
        : IRmQuery<KkListGetReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
    }

    public class KkListGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListGetReply()
        {
        }
    }
}
