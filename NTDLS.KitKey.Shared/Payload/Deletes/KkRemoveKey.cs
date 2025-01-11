using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.Deletes
{
    /// <summary>
    /// Removes a key from a key-store of any type.
    /// </summary>
    public class KkRemoveKey(string storeKey, string valueKey)
        : IRmQuery<KkRemoveKeyReply>
    {
        public string StoreKey { get; set; } = storeKey;
        public string ValueKey { get; set; } = valueKey;
    }

    public class KkRemoveKeyReply
        : IRmQueryReply
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public KkRemoveKeyReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        public KkRemoveKeyReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        public KkRemoveKeyReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public KkRemoveKeyReply()
        {
        }
    }
}
