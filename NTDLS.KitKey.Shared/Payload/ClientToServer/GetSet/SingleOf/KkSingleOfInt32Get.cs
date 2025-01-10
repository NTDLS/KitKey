using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfInt32Get(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfInt32GetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfInt32GetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public int? Value { get; set; }

        public KkSingleOfInt32GetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfInt32GetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfInt32GetReply()
        {
        }
    }
}
