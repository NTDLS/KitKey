﻿using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfInt32
{
    /// <summary>
    /// API payload used to get a single value from a key-store.
    /// </summary>
    public class KkSingleOfInt32Get(string storeKey, string valueKey)
        : IRmQuery<KkSingleOfInt32GetReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ValueKey { get; set; } = valueKey;
    }

    /// <summary>
    /// API payload used to get a single value from a key-store.
    /// </summary>
    public class KkSingleOfInt32GetReply
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
        /// The value retrieved from the key-value store for the given ValueKey.
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Throws the ErrorMessage where IsSuccess is not true.
        /// </summary>
        public KkSingleOfInt32GetReply EnsureSuccessful()
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
        public KkSingleOfInt32GetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfInt32GetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfInt32GetReply()
        {
        }
    }
}
