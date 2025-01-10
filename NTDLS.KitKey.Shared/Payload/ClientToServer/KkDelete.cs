using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ClientToServer
{
    /// <summary>
    /// Deletes a value of any type from a key-store. 
    /// </summary>
    public class KkDelete(string storeKey, string valueKey)
        : IRmQuery<KkDeleteReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkDeleteReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkDeleteReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkDeleteReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkDeleteReply()
        {
        }
    }
}
