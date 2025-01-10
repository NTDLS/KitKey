using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    /// <summary>
    /// Deletes a value of any type from a key-store. 
    /// </summary>
    public class KkDeleteKey(string storeKey, string valueKey)
        : IRmQuery<KkDeleteKeyReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkDeleteKeyReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkDeleteKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkDeleteKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkDeleteKeyReply()
        {
        }
    }
}
