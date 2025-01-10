using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfGuid
{
    public class KkListOfGuidGetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfGuidGetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfGuidGetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, Guid>? List { get; set; }

        public KkListOfGuidGetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfGuidGetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfGuidGetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfGuidGetAllReply()
        {
        }
    }
}
