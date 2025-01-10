using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt64PushLast(string storeKey, string listKey, long listValue)
        : IRmQuery<KkListOfInt64PushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public long ListValue { get; set; } = listValue;
    }

    public class KkListOfInt64PushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt64PushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64PushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64PushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64PushLastReply()
        {
        }
    }
}
