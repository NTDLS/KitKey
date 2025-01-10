using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer.GetSet.ListOf
{
    /// <summary>
    /// Appends a value to a string list key-store.
    /// </summary>
    public class KkListOfDateTimeAdd(string storeKey, string listKey, DateTime listValue)
        : IRmQuery<KkListOfDateTimeAddReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ListKey { get; set; } = listKey;
        public DateTime ListValue { get; set; } = listValue;
    }

    public class KkListOfDateTimeAddReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkListOfDateTimeAddReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkListOfDateTimeAddReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkListOfDateTimeAddReply()
        {
        }
    }
}
