using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStringSet(string storeKey, string valueKey, string value)
        : IRmQuery<KkStringSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
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
