using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfDoubleAdd(string storeKey, string listKey, Double listValue)
        : IRmQuery<KkListOfDoubleAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Double ListValue { get; set; } = listValue;
    }

    public class KkListOfDoubleAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDoubleAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDoubleAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDoubleAddReply()
        {
        }
    }
}
