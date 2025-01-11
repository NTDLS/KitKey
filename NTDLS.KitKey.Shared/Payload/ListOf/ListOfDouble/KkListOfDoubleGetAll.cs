using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDouble
{
    public class KkListOfDoubleGetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfDoubleGetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDoubleGetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<KkListItem<Double>>? List { get; set; }

        public KkListOfDoubleGetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDoubleGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoubleGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoubleGetAllReply()
        {
        }
    }
}
