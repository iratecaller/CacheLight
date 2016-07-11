using System;
using System.Collections.Generic;
using System.Linq;

namespace CacheLight
{


  

    public class Cache<TKey, TValue> : IDisposable, CacheLight.ICache<TKey,TValue>
    {
        public static int DefaultCacheSize = 1024;
        public event CacheEvictionHandler<TKey, TValue> OnCacheEviction;
        public event CacheItemLoadHandler<TKey, TValue> OnLoadItem;
        public event CacheItemSaveHandler<TKey, TValue> OnSaveItem;

        
        
        private Dictionary<TKey, CacheItem<TValue>> dict;
        private List<TKey> list;
        private int size;


        public Cache(int size)
        {
            Reset(size);
        }

        ~Cache()
        {
            Flush();
        }

        public TValue this[TKey key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            CacheItem<TValue> check;

            if (dict.TryGetValue(key, out check))
            {
                // overwrite.
                if (!check.Value.Equals(value))
                    check.Changed = true;
                check.Value = value;

                SetAsMostRecentlyUsed(key);
            }
            else
            {
                // are we full?
                EvictIfNecessary();

                // new item
                var CI = new CacheItem<TValue>();
                CI.Value = value;
                CI.Changed = false;

                // insert
                dict[key] = CI;
                list.Add(key);
            }
        }

        public void Dispose()
        {
            Flush();
        }

        public void Flush()
        {
            if (list.Count > 0)
                foreach (TKey key in list)
                    FlushItem(key);

            list.Clear();
            dict.Clear();
        }

        public TValue Get(TKey key)
        {
            TValue ret = default(TValue);
            CacheItem<TValue> V;
            if (dict.TryGetValue(key, out V))
            {
                SetAsMostRecentlyUsed(key);
                ret = V.Value;
            }
            else
            {
                if (OnLoadItem != null)
                {
                    TValue value;
                    if (OnLoadItem(key, out value))
                    {
                        Add(key, value);
                        ret = value;
                    }
                    else
                    {
                        throw new Exception("Load failed for item: " + key);
                    }
                }
            }

            return ret;
        }

        public bool Remove(TKey key)
        {
            FlushItem(key);
            if (dict.Remove(key))
            {
                list.Remove(key);
                return true;
            }
            else
                return false;
        }

        public void Reset(int size)
        {
            this.size = size;
            dict = new Dictionary<TKey, CacheItem<TValue>>(size);
            list = new List<TKey>(size);
        }
        private void EvictIfNecessary()
        {
            if (list.Count >= size)
            {
                TKey evict_head = list[0];
                FlushItem(evict_head);
                dict.Remove(evict_head);
                list.RemoveAt(0);
            }
        }

        private void FlushItem(TKey evict_head)
        {
            if (OnSaveItem != null || OnCacheEviction != null)
            {
                CacheItem<TValue> evict_value = dict[evict_head];
                TValue value = evict_value.Value;
                if (OnCacheEviction != null)
                {
                    OnCacheEviction(evict_head, ref evict_value);
                }
                if (evict_value.Changed && OnSaveItem != null)
                {
                    OnSaveItem(evict_head, ref value);
                    evict_value.Changed = false;
                }
            }
        }

        void SetAsMostRecentlyUsed(TKey key)
        {
            if (!list.Last().Equals(key))
            {
                if (list.Remove(key))
                {
                    list.Add(key);
                }
            }
        }
    }

   
}