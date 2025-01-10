using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfString
{
    public class KkListOfStringGetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfStringGetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfStringGetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, string>? Value { get; set; }

        public KkListOfStringGetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringGetFirstReply()
        {
        }
    }
}
