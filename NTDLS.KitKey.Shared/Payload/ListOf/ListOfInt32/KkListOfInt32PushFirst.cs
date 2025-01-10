using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt32PushFirst(string storeKey, string listKey, int listValue)
        : IRmQuery<KkListOfInt32PushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public int ListValue { get; set; } = listValue;
    }

    public class KkListOfInt32PushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt32PushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32PushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32PushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32PushFirstReply()
        {
        }
    }
}
