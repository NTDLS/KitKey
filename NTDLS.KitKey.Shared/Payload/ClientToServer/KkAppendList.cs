using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkAppendList(string storeName, string key, string value)
        : IRmQuery<KkAppendListReply>
    {
        public string StoreName { get; set; } = storeName;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public class KkAppendListReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkAppendListReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkAppendListReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkAppendListReply()
        {
        }
    }
}
