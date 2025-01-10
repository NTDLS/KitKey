using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64
{
    public class KkListOfInt64GetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfInt64GetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt64GetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, long>? Value { get; set; }

        public KkListOfInt64GetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64GetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64GetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64GetFirstReply()
        {
        }
    }
}
