using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListAdd(string storeName, string key, string value)
        : IRmQuery<KkListAddReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public class KkListAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListAddReply()
        {
        }
    }
}
