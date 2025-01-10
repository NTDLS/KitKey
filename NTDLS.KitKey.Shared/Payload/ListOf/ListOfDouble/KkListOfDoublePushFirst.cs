using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfDoublePushFirst(string storeKey, string listKey, double listValue)
        : IRmQuery<KkListOfDoublePushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public double ListValue { get; set; } = listValue;
    }

    public class KkListOfDoublePushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDoublePushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDoublePushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoublePushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoublePushFirstReply()
        {
        }
    }
}
