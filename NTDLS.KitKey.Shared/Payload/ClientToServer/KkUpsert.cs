using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkUpsert(string storeName, string key, string value)
        : IRmQuery<KkUpsertReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public class KkUpsertReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkUpsertReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkUpsertReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkUpsertReply()
        {
        }
    }
}
