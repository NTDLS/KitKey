﻿using NTDLS.ReliableMessaging;

namespace NTDLS.KitKey.Shared.Payload.ListOf.ListOfSingle
{
    /// <summary>
    /// API payload used to prepend a value to a list type key-value store.
    /// </summary>
    public class KkListOfSinglePushFirst(string storeKey, string listKey, float listValue)
        : IRmQuery<KkListOfSinglePushFirstReply>
    {
        /// <summary>
        /// The name (or identifier) for a key-value store.
        /// </summary>
        public string StoreKey { get; set; } = storeKey;

        /// <summary>
        /// The key (or identifier) of the value in the key-value store.
        /// </summary>
        public string ListKey { get; set; } = listKey;

        /// <summary>
        /// The value add to the list inside the key-value store for the given ListKey.
        /// </summary>
        public float ListValue { get; set; } = listValue;
    }

    /// <summary>
    /// API payload used to prepend a value to a list type key-value store.
    /// </summary>
    public class KkListOfSinglePushFirstReply
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
        public KkListOfSinglePushFirstReply EnsureSuccessful()
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
        public KkListOfSinglePushFirstReply(Exception exception)
        {
            IsSuccess = false;
            ErrorMessage = exception.Message;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfSinglePushFirstReply(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public KkListOfSinglePushFirstReply()
        {
        }
    }
}
