using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid
{
    public class KkListOfGuidGetFirst(string storeKey, string listKey)
        : IRmQuery<KkListOfGuidGetFirstReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfGuidGetFirstReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KkListItem<Guid>? Value { get; set; }

        public KkListOfGuidGetFirstReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidGetFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidGetFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidGetFirstReply()
        {
        }
    }
}
