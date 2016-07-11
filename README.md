# CacheLight
Unobtrusive portable cache for games and business applications.

## Nuget Info

Install-Package CacheLight

## Why?

Developers frequently encounter the need to cache information within their applications. Many third party caching systems are simply too
large and feature-rich for certain scenarios. Complex cache eviction policies and the need to be as flexible as possible, often leads the 
"big boy" cache systems to being slower and having a higher performance footprint than desired. (NOTE: This doesn't mean these other systems
are "BAD" , quite the contrary actually.)

In many cases a simple cache which limits the number of items it contains is sufficient. Sometimes there is no need for complex 
eviction policies.  


## How does CacheLight work?

Think of it as a dictionary that has a fixed size.  As items are inserted and the dictionary gets full, items that haven't 
been accessed in a while are removed to make room for the new items. 


## Features

- Portable Class Library (can work with mono on Android, IOS, Windows and Windows Phone)
- Optional cache eviction notification
- Optional OnLoad/OnSave events for cache items
- Single threaded, and friendly to game developers

## Why is this better than "System.Caching" , or "Arnold's Super Cache 2000"?

It isn't. It is simply more compact, single threaded, and does not use eviction policies. CacheLight won't make everybody happy
all of the time, but will make some people happy most of the time.

## Typical Uses

- A game wants to cache level information to avoid expensive loads and mid-game stutters, but the entire level is too big to keep in memory.
- Cache expensive database results
- Cache lists of values for pull downs
- Cache binary image data and textures 
- Automatically manage loading of map data using coordinates as keys
- Search acceleration structures and planning
- In memory cache for static web content
- Good for games, since it's single threaded


## Limitations

- CacheLight does not monitor memory usage and simply allows the cache to store at most N items. CacheLight is therefore not designed
as a building block for a large capacity cache or distributed cache. You might want to use the .NET cache, or memcached for that.


## Usage Modes

There are generally 2 ways to use CacheLight

### 1. Standard Dictionary Style Access

Use CacheLight as you would a dictionary. Example:    

    item = cache[key];  
    cache[other_key] =  other_item;

In this mode of operation,  item = cache[key] may return null to indicate that the item does not exist in the cache. Simply
insert it and it will be there until it is evicted again.

This is a typical mode of operation that enhances application performance. When the cache item is null, the application must 
perform necessary operations to load the item and insert it into the cache.

### 2. On-The-Fly Transparent Loading / Saving

When specifying a routine to handle the OnLoadItem and OnSaveItem, then CacheLight will automatically let your application know
when it's time to do something.  

Example: Transparent Loading:  

The application performs  "image = image_cache["boots.png"];". Even though "boots.png" hasn't been loaded, image will NOT contain null. 
CacheLight will call your OnLoadItem event, and your application will perform the load.  
    
     image_cache.OnLoadItem += LoadImage;
     var image = image_cache["boots.png"]; // image contains the boots.png image data
     ...
    bool LoadImage(string key, out object item)
    {
       item = LoadImageFromFile(ImagePath + key);
       return true; // succesfully loaded.
    }

## Detailed Example

What follows is a full C# example:

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



