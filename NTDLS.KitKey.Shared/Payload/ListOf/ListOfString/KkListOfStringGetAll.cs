using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfString
{
    public class KkListOfStringGetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfStringGetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfStringGetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public List<KkListItem<string>>? List { get; set; }

        public KkListOfStringGetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringGetAllReply()
        {
        }
    }
}
