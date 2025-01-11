using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime
{
    public class KkListOfDateTimeGetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfDateTimeGetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDateTimeGetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<KkListItem<DateTime>>? List { get; set; }

        public KkListOfDateTimeGetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDateTimeGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimeGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimeGetAllReply()
        {
        }
    }
}
