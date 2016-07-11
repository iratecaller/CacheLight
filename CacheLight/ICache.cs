namespace CacheLight
{
    public interface ICache<TKey, TValue>
    {
        event CacheEvictionHandler<TKey, TValue> OnCacheEviction;

        event CacheItemLoadHandler<TKey, TValue> OnLoadItem;

        event CacheItemSaveHandler<TKey, TValue> OnSaveItem;

        TValue this[TKey key] { get; set; }

        void Add(TKey key, TValue value);
        void Dispose();
        void Flush();
        TValue Get(TKey key);
        bool Remove(TKey key);
        void Reset(int size);
    }
}
