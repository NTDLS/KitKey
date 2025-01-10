using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid
{
    public class KkListOfGuidGetLast(string storeKey, string listKey)
        : IRmQuery<KkListOfGuidGetLastReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfGuidGetLastReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public KeyValuePair<Guid, Guid>? Value { get; set; }

        public KkListOfGuidGetLastReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidGetLastReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidGetLastReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidGetLastReply()
        {
        }
    }
}
