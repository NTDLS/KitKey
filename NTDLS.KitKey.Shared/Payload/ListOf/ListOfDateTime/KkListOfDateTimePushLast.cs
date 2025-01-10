using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfDateTimePushLast(string storeKey, string listKey, DateTime listValue)
        : IRmQuery<KkListOfDateTimePushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public DateTime ListValue { get; set; } = listValue;
    }

    public class KkListOfDateTimePushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDateTimePushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDateTimePushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimePushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimePushLastReply()
        {
        }
    }
}
