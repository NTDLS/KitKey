using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    public class KkListOfSingleGetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfSingleGetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfSingleGetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, float>? Value { get; set; }

        public KkListOfSingleGetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfSingleGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSingleGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSingleGetFirstReply()
        {
        }
    }
}
