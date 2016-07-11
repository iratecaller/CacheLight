using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheLight;

namespace CacheLightDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var MyCache = new Cache<string, object>(10);

            // optional event handlers.

            // item evited from cache
            MyCache.OnCacheEviction += MyCache_OnCacheEviction;
            
            // item should be saved (cache detected that the item has changed)
            MyCache.OnSaveItem += MyCache_OnSaveItem;

            // someone attemped to get MyCache[x], and x is currently not loaded.
            // normally null is returned, and this is typical behavior
            // however, OnLoadItem allows the program to intercept the request
            // and perform the load. 
            MyCache.OnLoadItem += MyCache_OnLoadItem;
            
            // load the cache
            for(int i=0;i<20;i++)
            {
                MyCache["Item " + i] = i;
            }

            // attempt to access something that isn't cached:

            var item = MyCache["Item 9999"];
            Console.WriteLine("Retrieved: " + item);


            MyCache.Flush();
            Console.WriteLine("Press enter to finish ...");
            Console.ReadLine();

        }

        static bool MyCache_OnLoadItem(string key, out object item)
        {
            Console.WriteLine("Item " + key + " accesed but isn't in the cache. Loading it ...");
            // pretend that 1000 was loaded as the item that maps to the key.
            item = 1000;
            return true; // succesfully loaded.
        }

        static void MyCache_OnSaveItem(string key, ref object item)
        {
            Console.WriteLine("Cache must evict: " + key + " and changes have been detected.");
            // save work.

        }

        static void MyCache_OnCacheEviction(string key, ref CacheItem<object> item)
        {
            Console.WriteLine("Evicted " + key);
        }
    }
}
