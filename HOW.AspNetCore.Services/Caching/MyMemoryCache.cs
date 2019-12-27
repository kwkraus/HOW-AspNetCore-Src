using Microsoft.Extensions.Caching.Memory;

namespace HOW.AspNetCore.Services.Caching
{
    // using Microsoft.Extensions.Caching.Memory;
    public class MyMemoryCache
    {
        public MemoryCache Cache { get; private set; }
        public MyMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
    }
}
