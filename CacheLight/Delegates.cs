
namespace CacheLight
{
    public delegate void CacheEvictionHandler<TKey, TValue>(TKey key, ref CacheItem<TValue> item);
    public delegate bool CacheItemLoadHandler<TKey, TValue>(TKey key, out TValue item);
    public delegate void CacheItemSaveHandler<TKey, TValue>(TKey key, ref TValue item);
}
