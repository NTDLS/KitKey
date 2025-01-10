using NTDLS.Helpers;
using System.Text.Json.Serialization;

namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// Defines a key store configuration.
    /// </summary>
    public class KkStoreConfiguration
    {
        private string? _storeName;
        /// <summary>
        /// The name of the key-store.
        /// </summary>
        public string StoreKey
        {
            get => _storeName.EnsureNotNull();
            set => _storeName = value;
        }

        /// <summary>
        /// Whether the key-store is persisted or ephemeral.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KkPersistenceScheme PersistenceScheme { get; set; } = KkPersistenceScheme.Ephemeral;

        /// <summary>
        /// Specifies the type of values or values that the store will contain.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public KkValueType ValueType { get; set; } = KkValueType.String;

        /// <summary>
        /// The amount of (sliding expiration) time that a key/value should stay in cache. If not defined, persistent
        /// key-stores will use KkDefaults.DEFAULT_CACHE_SECONDS while ephemeral key-stores will be 1 day (since they
        /// live solely in cache). Note that defining an expiration for ephemeral key-stores will result in loss of the
        /// values once they expire.
        /// </summary>
        public TimeSpan? CacheExpiration { get; set; }

        /// <summary>
        /// Instantiates a new instance of KkStoreConfiguration.
        /// </summary>
        public KkStoreConfiguration()
        {
        }

        /// <summary>
        /// Instantiates a new instance of KkStoreConfiguration.
        /// </summary>
        public KkStoreConfiguration(string storeKey)
        {
            StoreKey = storeKey;
        }
    }
}
