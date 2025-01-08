namespace NTDLS.KitKey.Server.Server
{
    internal class CaseInsensitiveMessageQueueDictionary : Dictionary<string, MessageQueue>
    {
        public CaseInsensitiveMessageQueueDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
