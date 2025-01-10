using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfInt32
{
    public class KkListOfInt32GetAll(string storeKey, string listKey)
        : IRmQuery<KkListOfInt32GetAllReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt32GetAllReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListOfInt32GetAllReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt32GetAllReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt32GetAllReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt32GetAllReply()
        {
        }
    }
}
