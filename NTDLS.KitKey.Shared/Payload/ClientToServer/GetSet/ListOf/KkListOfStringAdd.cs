using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfStringAdd(string storeKey, string listKey, string listValue)
        : IRmQuery<KkListOfStringAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public string ListValue { get; set; } = listValue;
    }

    public class KkListOfStringAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfStringAddReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringAddReply()
        {
        }
    }
}
