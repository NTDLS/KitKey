using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfInt64Get(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfInt64GetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfInt64GetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public long? Value { get; set; }

        public KkSingleOfInt64GetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfInt64GetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfInt64GetReply()
        {
        }
    }
}
