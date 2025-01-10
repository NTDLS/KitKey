using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Deletes
{
    /// <summary>
    /// Removes a key from a key-store of any type.
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

        public KkDeleteKeyReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

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
