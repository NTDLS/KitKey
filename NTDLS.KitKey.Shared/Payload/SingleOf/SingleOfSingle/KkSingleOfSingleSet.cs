using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfSingle
{
    public class KkSingleOfSingleSet(string storeKey, string valueKey, float value)
        : IRmQuery<KkSingleOfSingleSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public float Value { get; set; } = value;
    }

    public class KkSingleOfSingleSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfSingleSetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

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
