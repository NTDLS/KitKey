using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid
{
    /// <summary>
    /// Appends a value to a Guid list key-store.
    /// </summary>
    public class KkListOfGuidPushLast(string storeKey, string listKey, Guid listValue)
        : IRmQuery<KkListOfGuidPushLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Guid ListValue { get; set; } = listValue;
    }

    public class KkListOfGuidPushLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfGuidPushLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidPushLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidPushLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidPushLastReply()
        {
        }
    }
}
