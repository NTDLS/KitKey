using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfInt64Set(string storeKey, string valueKey, long value)
        : IRmQuery<KkSingleOfInt64SetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public long Value { get; set; } = value;
    }

    public class KkSingleOfInt64SetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfInt64SetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfInt64SetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfInt64SetReply()
        {
        }
    }
}
