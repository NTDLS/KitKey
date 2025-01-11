using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt64
{
    public class KkListOfInt64GetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfInt64GetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt64GetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<KkListItem<Int64>>? List { get; set; }

        public KkListOfInt64GetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64GetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64GetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64GetAllReply()
        {
        }
    }
}
