using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfInt64Add(string storeKey, string listKey, Int64 listValue)
        : IRmQuery<KkListOfInt64AddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Int64 ListValue { get; set; } = listValue;
    }

    public class KkListOfInt64AddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfInt64AddReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64AddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64AddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64AddReply()
        {
        }
    }
}
