using NTDLS.Helpers;
using System.Text.Json.Serialization;

namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// Defines a key store configuration.
    /// </summary>
    public class CMqStoreConfiguration
    {
        private string? _storeName;
        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string StoreName
        {
            get => _storeName.EnsureNotNull();
            set => _storeName = value;
        }

        /// <summary>
        /// Whether the queue is persisted or ephemeral.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CMqPersistenceScheme PersistenceScheme { get; set; } = CMqPersistenceScheme.Ephemeral;

        /// <summary>
        /// Instantiates a new instance of CMqQueueConfiguration.
        /// </summary>
        public CMqStoreConfiguration()
        {
        }

        /// <summary>
        /// Instantiates a new instance of CMqQueueConfiguration.
        /// </summary>
        public CMqStoreConfiguration(string storeName)
        {
            StoreName = storeName;
        }
    }
}
