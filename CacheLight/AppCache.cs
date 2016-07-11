
namespace CacheLight
{
    public class CacheFactory
    {
        private static Cache<object, object> _default;

        public static ICache<object, object> Default
        {
            get
            {
                if (_default == null)
                    _default = new Cache<object, object>(Cache<object, object>.DefaultCacheSize);
                return _default;
            }
        }

    }
}
