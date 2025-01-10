using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfStringSet(string storeKey, string valueKey, string value)
        : IRmQuery<KkSingleOfStringSetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public string Value { get; set; } = value;
    }

    public class KkSingleOfStringSetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfStringSetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfStringSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfStringSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfStringSetReply()
        {
        }
    }
}
