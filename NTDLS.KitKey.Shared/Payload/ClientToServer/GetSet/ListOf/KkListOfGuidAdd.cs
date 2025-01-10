using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a Guid list key-store.
    /// </summary>
    public class KkListOfGuidAdd(string storeKey, string listKey, Guid listValue)
        : IRmQuery<KkListOfStringAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Guid ListValue { get; set; } = listValue;
    }

    public class KkListOfGuidAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfGuidAddReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidAddReply()
        {
        }
    }
}
