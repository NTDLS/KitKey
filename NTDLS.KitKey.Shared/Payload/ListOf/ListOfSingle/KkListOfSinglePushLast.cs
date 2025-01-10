using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfSinglePushLast(string storeKey, string listKey, float listValue)
        : IRmQuery<KkListOfSinglePushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public float ListValue { get; set; } = listValue;
    }

    public class KkListOfSinglePushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfSinglePushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfSinglePushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSinglePushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSinglePushLastReply()
        {
        }
    }
}
