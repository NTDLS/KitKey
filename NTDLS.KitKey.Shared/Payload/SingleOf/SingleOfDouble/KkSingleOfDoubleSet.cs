﻿using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.SingleOf.SingleOfDouble
{
    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfDoubleSet(string storeKey, string valueKey, double value)
        : IRmQuery<KkSingleOfDoubleSetReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;
        /// <summary>
        /// 
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ValueKey { get; set; } = valueKey;

        /// <summary>
        /// The value to insert or update to the key-value store for the given ValueKey.
        /// </summary>
        public double Value { get; set; } = value;
    }

    /// <summary>
    /// API payload used to insert or update a single value in a key-store.
    /// </summary>
    public class KkSingleOfDoubleSetReply
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
        public KkSingleOfDoubleSetReply EnsureSuccessful()
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
        public KkSingleOfDoubleSetReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfDoubleSetReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkSingleOfDoubleSetReply()
        {
        }
    }
}
