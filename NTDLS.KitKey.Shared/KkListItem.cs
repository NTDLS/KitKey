namespace NTDLS.KitKey.Shared
{
    /// <summary>
    /// API payload used as an envelope for key-value store list items.
    /// </summary>
    public class KkListItem<T>
    {
        private T? _value;

        /// <summary>
        /// Unique server side generated identity for the list item.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List item value.
        /// </summary>
        public T Value
        {
            get
            {
                return _value ?? throw new Exception("List values can not be null.");
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the a list item
        /// </summary>
        public KkListItem()
        {
        }

        /// <summary>
        /// /// Creates a new instance of the a list item
        /// </summary>
        public KkListItem(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new instance of the a list item
        /// </summary>
        public KkListItem(Guid id, T value)
        {
            Id = id;
            _value = value;
        }
    }
}
