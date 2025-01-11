using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    public class KkListOfSingleGetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfSingleGetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfSingleGetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<float>? Value { get; set; }

        public KkListOfSingleGetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfSingleGetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSingleGetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSingleGetLastReply()
        {
        }
    }
}
