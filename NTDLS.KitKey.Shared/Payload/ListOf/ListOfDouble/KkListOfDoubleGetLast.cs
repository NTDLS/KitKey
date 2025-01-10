using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble
{
    public class KkListOfDoubleGetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfDoubleGetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDoubleGetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, double>? Value { get; set; }

        public KkListOfDoubleGetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDoubleGetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoubleGetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoubleGetLastReply()
        {
        }
    }
}
