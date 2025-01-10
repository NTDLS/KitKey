using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.SingleOf
{
    public class KkSingleOfDateTimeGet(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfDateTimeGetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkSingleOfDateTimeGetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? Value { get; set; }

        public KkSingleOfDateTimeGetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkSingleOfDateTimeGetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkSingleOfDateTimeGetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkSingleOfDateTimeGetReply()
        {
        }
    }
}
