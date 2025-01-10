using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt64PushFirst(string storeKey, string listKey, long listValue)
        : IRmQuery<KkListOfInt64PushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public long ListValue { get; set; } = listValue;
    }

    public class KkListOfInt64PushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt64PushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64PushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64PushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64PushFirstReply()
        {
        }
    }
}
