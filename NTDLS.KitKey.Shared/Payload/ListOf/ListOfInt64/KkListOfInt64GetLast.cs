using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64
{
    public class KkListOfInt64GetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfInt64GetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt64GetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<long>? Value { get; set; }

        public KkListOfInt64GetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64GetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64GetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64GetLastReply()
        {
        }
    }
}
