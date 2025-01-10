using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfDateTimePushFirst(string storeKey, string listKey, DateTime listValue)
        : IRmQuery<KkListOfDateTimePushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public DateTime ListValue { get; set; } = listValue;
    }

    public class KkListOfDateTimePushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDateTimePushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDateTimePushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimePushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimePushFirstReply()
        {
        }
    }
}
