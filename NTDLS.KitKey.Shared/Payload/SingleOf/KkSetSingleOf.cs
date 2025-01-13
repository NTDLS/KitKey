
/* Unmerged change from project 'NTDLS.KitKey.Shared (net8.0)'
Before:
using NTDLS.ReliableMessaging;
After:
using NTDLS;
using NTDLS.KitKey;
using NTDLS.KitKey.Shared;
using NTDLS.KitKey.Shared.Payload;
using NTDLS.KitKey.Shared.Payload.SingleOf;
using NTDLS.KitKey.Shared.Payload.SingleOf;
using NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDateTime;
using NTDLS.ReliableMessaging;
*/
using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf
{
    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSetSingleOf<T>(string storeKey, string valueKey, T value)
        : IRmQuery<KkSetSingleOfReply>
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
        public T Value { get; set; } = value;
    }

    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSetSingleOfReply
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
        public KkSetSingleOfReply EnsureSuccessful()
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
        public KkSetSingleOfReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSetSingleOfReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSetSingleOfReply()
        {
        }
    }
}
