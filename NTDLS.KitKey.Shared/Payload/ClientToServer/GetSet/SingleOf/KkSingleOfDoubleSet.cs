using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfDoubleSet(string storeKey, string valueKey, double value)
        : IRmQuery<KkSingleOfDoubleSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public double Value { get; set; } = value;
    }

    public class KkSingleOfDoubleSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfDoubleSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfDoubleSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfDoubleSetReply()
        {
        }
    }
}
