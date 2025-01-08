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
        public string StoreName
        {
            get => _storeName.EnsureNotNull();
            set => _storeName = value;
        }

        /// <summary>
        /// Whether the key-store is persisted or ephemeral.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CMqPersistenceScheme PersistenceScheme { get; set; } = CMqPersistenceScheme.Ephemeral;

        /// <summary>
        /// Instantiates a new instance of KkStoreConfiguration.
        /// </summary>
        public KkStoreConfiguration()
        {
        }

        /// <summary>
        /// Instantiates a new instance of KkStoreConfiguration.
        /// </summary>
        public KkStoreConfiguration(string storeName)
        {
            StoreName = storeName;
        }
    }
}
