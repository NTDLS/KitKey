using ProtoBuf;

namespace NTDLS.KitKey.Server.Server
{
    /// <summary>
    /// Internal envelope for a key-store list item.
    /// </summary>

    [ProtoContract]
    internal class KkListItem
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public byte[] Bytes { get; set; }

        public KkListItem()
        {
            Bytes = new byte[0];
        }

        public KkListItem(byte[] value)
        {
            Id = Guid.NewGuid();
            Bytes = value;
        }
    }
}
