namespace NTDLS.KitKey.Shared
{
    public class KkListItem<T>
    {
        private T? _value;

        public Guid Id { get; set; }
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

        public KkListItem()
        {
        }

        public KkListItem(T value)
        {
            _value = value;
        }

        public KkListItem(Guid id, T value)
        {
            Id = id;
            _value = value;
        }
    }
}
