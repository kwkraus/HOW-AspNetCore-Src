using HOW.AspNetCore.Services.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace HOW.AspNetCore.Razor.WebApp
{
    public class TestCacheModel : PageModel
    {
        private readonly MemoryCache _cache;
        public static readonly string MyKey = "_MyKey";

        public TestCacheModel(MyMemoryCache memoryCache)
        {
            _cache = memoryCache.Cache;
        }

        [TempData]
        public string DateTime_Now { get; set; }
        [TempData]
        public int Cache_size { get; set; }

        #region snippet2
        public IActionResult OnGet()
        {
            if (!_cache.TryGetValue(MyKey, out string cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = DateTime.Now.TimeOfDay.ToString();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Set cache entry size by extension method.
                    .SetSize(1)
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(3));

                // Set cache entry size via property.
                // cacheEntryOptions.Size = 1;

                // Save data in cache.
                _cache.Set(MyKey, cacheEntry, cacheEntryOptions);
            }

            DateTime_Now = cacheEntry;
            Cache_size = _cache.Count;

            return Page();
        }
        #endregion


        public IActionResult OnPost()
        {
            #region snippet3       
            _cache.Remove(MyKey);

            // Remove 33% of cached items.
            _cache.Compact(.33);
            Cache_size = _cache.Count;
            #endregion
            return Page();
        }
    }
}