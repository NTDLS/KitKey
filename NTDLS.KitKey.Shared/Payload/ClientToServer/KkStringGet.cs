using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    public class KkStringGet(string storeKey, string valueKey)
        : IRmQuery<KkStringGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
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
