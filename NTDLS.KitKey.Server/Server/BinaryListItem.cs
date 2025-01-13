using ProtoBuf;

namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// Internal envelope for a key-store list item.
    /// </summary>

    [ProtoContract]
    internal class BinaryListItem
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public byte[]? Bytes { get; set; }

        public BinaryListItem()
        {
        }

        public BinaryListItem(Guid id, byte[] value)
        {
            Id = id;
            Bytes = value;
        }
    }
}
