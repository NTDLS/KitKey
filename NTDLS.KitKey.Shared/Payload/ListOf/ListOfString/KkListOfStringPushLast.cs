using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfString
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfStringPushLast(string storeKey, string listKey, string listValue)
        : IRmQuery<KkListOfStringPushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public string ListValue { get; set; } = listValue;
    }

    public class KkListOfStringPushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfStringPushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringPushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringPushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringPushLastReply()
        {
        }
    }
}
