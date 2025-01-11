using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    public class KkListOfSingleGetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfSingleGetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfSingleGetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<KkListItem<Single>>? List { get; set; }

        public KkListOfSingleGetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfSingleGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSingleGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSingleGetAllReply()
        {
        }
    }
}
