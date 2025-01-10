namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// Whether the key-store is persisted or ephemeral.
    /// </summary>
    public enum KkPersistenceScheme
    {
        /// <summary>
        /// Key-values are cached and removed when the service restarts.
        /// </summary>
        Ephemeral,
        /// <summary>
        /// Key-values are stored on disk and survive service restarts.
        /// </summary>
        Persistent
    }

    /// <summary>
    /// Specifies the format in which the value is to managed.
    /// </summary>
    public enum KkValueType
    {
        /// <summary>
        /// Value is stored as a single string.
        /// </summary>
        String,
        /// <summary>
        /// Value is stored as a single int32.
        /// </summary>
        Int32,
        /// <summary>
        /// Value is stored as a single int64.
        /// </summary>
        Int64,
        /// <summary>
        /// Value is stored as a single double.
        /// </summary>
        Double,
        /// <summary>
        /// Value is stored as a single float.
        /// </summary>
        Float,
        /// <summary>
        /// Value is stored as a single DateTime.
        /// </summary>
        DateTime,
        /// <summary>
        /// Value is stored as a list of strings.
        /// </summary>
        StringList,
        /// <summary>
        /// Value is stored as a list of int32s.
        /// </summary>
        Int32List,
        /// <summary>
        /// Value is stored as a list of Int64s.
        /// </summary>
        Int64List,
        /// <summary>
        /// Value is stored as a list of Floats.
        /// </summary>
        FloatList,
        /// <summary>
        /// Value is stored as a list of Doubles.
        /// </summary>
        DoubleList,
        /// <summary>
        /// Value is stored as a list of DateTimes.
        /// </summary>
        DateTimeList
    }

    /// <summary>
    /// Used for message and error logging.
    /// </summary>
    public enum KkErrorLevel
    {
        /// <summary>
        /// Use for detailed diagnostic information.
        /// </summary>
        Verbose,
        /// <summary>
        /// Use for debugging information.
        /// </summary>
        Debug,
        /// <summary>
        /// Use for general informational messages.
        /// </summary>
        Information,
        /// <summary>
        /// Use for potentially harmful situations.
        /// </summary>
        Warning,
        /// <summary>
        ///Use for errors that prevent the execution of a specific part of the application.    
        /// </summary>
        Error,
        /// <summary>
        /// Use for critical errors that cause the application to crash or terminate.
        /// </summary>
        Fatal
    }
}
