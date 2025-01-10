using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfSingleAdd(string storeKey, string listKey, Single listValue)
        : IRmQuery<KkListOfSingleAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public Single ListValue { get; set; } = listValue;
    }

    public class KkListOfSingleAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfSingleAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfSingleAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfSingleAddReply()
        {
        }
    }
}
