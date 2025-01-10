using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfDoubleGet(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfDoubleGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfDoubleGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public double? Value { get; set; }

        public KkSingleOfDoubleGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfDoubleGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfDoubleGetReply()
        {
        }
    }
}
