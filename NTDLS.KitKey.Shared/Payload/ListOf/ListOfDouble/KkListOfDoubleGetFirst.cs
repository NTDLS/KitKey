using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble
{
    public class KkListOfDoubleGetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfDoubleGetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDoubleGetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<double>? Value { get; set; }

        public KkListOfDoubleGetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDoubleGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoubleGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoubleGetFirstReply()
        {
        }
    }
}
