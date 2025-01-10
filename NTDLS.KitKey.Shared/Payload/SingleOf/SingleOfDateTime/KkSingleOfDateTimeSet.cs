using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDateTime
{
    public class KkSingleOfDateTimeSet(string storeKey, string valueKey, DateTime value)
        : IRmQuery<KkSingleOfDateTimeSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public DateTime Value { get; set; } = value;
    }

    public class KkSingleOfDateTimeSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfDateTimeSetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfDateTimeSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfDateTimeSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfDateTimeSetReply()
        {
        }
    }
}
