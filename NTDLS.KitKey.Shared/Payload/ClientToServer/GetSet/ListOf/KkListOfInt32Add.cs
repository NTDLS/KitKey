using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt32Add(string storeKey, string listKey, Int32 listValue)
        : IRmQuery<KkListOfInt32AddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Int32 ListValue { get; set; } = listValue;
    }

    public class KkListOfInt32AddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt32AddReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32AddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32AddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32AddReply()
        {
        }
    }
}
