using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime
{
    public class KkListOfDateTimeGetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfDateTimeGetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDateTimeGetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<DateTime>? Value { get; set; }

        public KkListOfDateTimeGetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDateTimeGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimeGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimeGetFirstReply()
        {
        }
    }
}
