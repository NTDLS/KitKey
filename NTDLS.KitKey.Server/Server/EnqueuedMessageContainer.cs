using RocksDbSharp;

namespace NTDLS.KitKey.Server.Server
{
    internal class EnqueuedMessageContainer
    {
        public List<EnqueuedMessage> Messages { get; set; } = new();

        public RocksDb? Database { get; set; }
    }
}
