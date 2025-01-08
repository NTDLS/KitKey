using NTDLS.KitKey.Shared;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// We use this class to save a queues meta-data to disk.
    /// </summary>
    internal class MessageQueueMetadata
    {
        public CMqStoreConfiguration Configuration { get; set; } = new();
        public MessageQueueStatistics Statistics { get; set; } = new();

        public MessageQueueMetadata(CMqStoreConfiguration configuration, MessageQueueStatistics statistics)
        {
            Configuration = configuration;
            Statistics = statistics;
        }

        public MessageQueueMetadata()
        {
        }
    }
}
