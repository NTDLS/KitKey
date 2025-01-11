using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfString
{
    public class KkListOfStringGetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfStringGetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfStringGetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<string>? Value { get; set; }

        public KkListOfStringGetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfStringGetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfStringGetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfStringGetLastReply()
        {
        }
    }
}
