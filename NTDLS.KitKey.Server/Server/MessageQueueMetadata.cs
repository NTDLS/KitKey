using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// We use this class to save a queues meta-data to disk.
    /// </summary>
    internal class MessageQueueMetadata
    {
        public KkStoreConfiguration Configuration { get; set; } = new();
        public MessageQueueStatistics Statistics { get; set; } = new();

        public MessageQueueMetadata(KkStoreConfiguration configuration, MessageQueueStatistics statistics)
        {
            Configuration = configuration;
            Statistics = statistics;
        }

        public MessageQueueMetadata()
        {
        }
    }
}
