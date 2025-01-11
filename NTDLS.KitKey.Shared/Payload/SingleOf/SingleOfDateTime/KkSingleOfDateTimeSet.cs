using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDateTime
{
    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfDateTimeSet(string storeKey, string valueKey, DateTime value)
        : IRmQuery<KkSingleOfDateTimeSetReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ValueKey { get; set; } = valueKey;

        /// <summary>
        /// The value to insert or update to the key-value store for the given ValueKey.
        /// </summary>
        public DateTime Value { get; set; } = value;
    }

    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfDateTimeSetReply
        : IRmQueryReply
    {
        /// <summary>
        /// Indicated whether the original query was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary> 
        /// When IsSuccess is not true, ErrorMessage indicates the error which occurred. 
        /// </summary> 
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Throws the ErrorMessage where IsSuccess is not true.
        /// </summary>
        public KkSingleOfDateTimeSetReply EnsureSuccessful()
        {
            if (!IsSuccess)
            {
                throw new Exception(ErrorMessage);
            }
            return this;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfDateTimeSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfDateTimeSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfDateTimeSetReply()
        {
        }
    }
}
