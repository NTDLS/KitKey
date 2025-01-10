using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfGuidGet(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfStringGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfGuidGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Value { get; set; }

        public KkSingleOfGuidGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfGuidGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfGuidGetReply()
        {
        }
    }
}
