using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt32PushLast(string storeKey, string listKey, int listValue)
        : IRmQuery<KkListOfInt32PushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public int ListValue { get; set; } = listValue;
    }

    public class KkListOfInt32PushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt32PushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32PushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32PushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32PushLastReply()
        {
        }
    }
}
