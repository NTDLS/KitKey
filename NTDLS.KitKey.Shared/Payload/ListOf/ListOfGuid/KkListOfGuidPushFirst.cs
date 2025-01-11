using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid
{
    /// <summary>
    /// Prepends a value to a Guid list key-store.
    /// </summary>
    public class KkListOfGuidPushFirst(string storeKey, string listKey, Guid listValue)
        : IRmQuery<KkListOfGuidPushFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Guid ListValue { get; set; } = listValue;
    }

    public class KkListOfGuidPushFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfGuidPushFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidPushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidPushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidPushFirstReply()
        {
        }
    }
}
