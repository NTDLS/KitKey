using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32
{
    public class KkListOfInt32GetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfInt32GetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt32GetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<int>? Value { get; set; }

        public KkListOfInt32GetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32GetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32GetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32GetLastReply()
        {
        }
    }
}
