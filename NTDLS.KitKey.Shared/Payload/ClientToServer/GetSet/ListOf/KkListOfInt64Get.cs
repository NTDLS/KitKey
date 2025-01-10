using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    public class KkListOfInt64Get(string storeKey, string listKey)
        : IRmQuery<KkListOfInt64GetReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
    }

    public class KkListOfInt64GetReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<Guid, string>? List { get; set; }

        public KkListOfInt64GetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkListOfInt64GetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfInt64GetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfInt64GetReply()
        {
        }
    }
}
