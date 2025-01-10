using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfInt32Set(string storeKey, string valueKey, int value)
        : IRmQuery<KkSingleOfInt32SetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
        public int Value { get; set; } = value;
    }

    public class KkSingleOfInt32SetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkSingleOfInt32SetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfInt32SetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfInt32SetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfInt32SetReply()
        {
        }
    }
}
