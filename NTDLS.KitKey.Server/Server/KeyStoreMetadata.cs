using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// We use this class to save a key-store's meta-data to disk.
    /// </summary>
    internal class KeyStoreMetadata
    {
        public KkStoreConfiguration Configuration { get; set; } = new();
        public KeyStoreStatistics Statistics { get; set; } = new();

        public KeyStoreMetadata(KkStoreConfiguration configuration, KeyStoreStatistics statistics)
        {
            Configuration = configuration;
            Statistics = statistics;
        }

        public KeyStoreMetadata()
        {
        }
    }
}
