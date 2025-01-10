using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    /// <summary>
    /// Prepends a value to a string list key-store.
    /// </summary>
    public class KkListOfSinglePushFirst(string storeKey, string listKey, float listValue)
        : IRmQuery<KkListOfSinglePushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public float ListValue { get; set; } = listValue;
    }

    public class KkListOfSinglePushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfSinglePushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfSinglePushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSinglePushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSinglePushFirstReply()
        {
        }
    }
}
