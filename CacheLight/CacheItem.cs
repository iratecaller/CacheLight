
namespace CacheLight
{
    public class CacheItem<T>
    {
        private T _value;

        public CacheItem()
        {
            _value = default(T);
            Changed = false;
        }

        public bool Changed
        {
            get;
            set;
        }
        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                Changed = true;
                _value = value;
            }
        }
    }
}
