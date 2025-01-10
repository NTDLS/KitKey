using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfDoublePushLast(string storeKey, string listKey, double listValue)
        : IRmQuery<KkListOfDoublePushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public double ListValue { get; set; } = listValue;
    }

    public class KkListOfDoublePushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDoublePushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDoublePushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoublePushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoublePushLastReply()
        {
        }
    }
}
