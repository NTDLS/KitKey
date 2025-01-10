using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfSingle
{
    public class KkSingleOfSingleGet(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfSingleGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfSingleGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public float? Value { get; set; }

        public KkSingleOfSingleGetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfSingleGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfSingleGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfSingleGetReply()
        {
        }
    }
}
