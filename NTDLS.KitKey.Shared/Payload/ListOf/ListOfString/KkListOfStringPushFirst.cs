using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfString
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfStringPushFirst(string storeKey, string listKey, string listValue)
        : IRmQuery<KkListOfStringPushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public string ListValue { get; set; } = listValue;
    }

    public class KkListOfStringPushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfStringPushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringPushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringPushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringPushFirstReply()
        {
        }
    }
}
