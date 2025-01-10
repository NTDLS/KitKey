using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32
{
    public class KkListOfInt32GetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfInt32GetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt32GetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, int>? Value { get; set; }

        public KkListOfInt32GetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32GetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32GetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32GetFirstReply()
        {
        }
    }
}
