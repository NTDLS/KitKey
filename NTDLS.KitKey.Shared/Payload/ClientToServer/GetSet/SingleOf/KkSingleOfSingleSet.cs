using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfSingleSet(string storeKey, string valueKey, Single value)
        : IRmQuery<KkSingleOfSingleSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public Single Value { get; set; } = value;
    }

    public class KkSingleOfSingleSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfSingleSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfSingleSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfSingleSetReply()
        {
        }
    }
}
