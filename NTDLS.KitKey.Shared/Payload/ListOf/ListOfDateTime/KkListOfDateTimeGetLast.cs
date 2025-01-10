using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfDateTime
{
    public class KkListOfDateTimeGetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfDateTimeGetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfDateTimeGetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, DateTime>? Value { get; set; }

        public KkListOfDateTimeGetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfDateTimeGetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimeGetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimeGetLastReply()
        {
        }
    }
}
