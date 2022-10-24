using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            //// First method of set data from memory
            //if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            //{
            //    _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            //}

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            
            options.SetPriority(CacheItemPriority.High);

            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _memoryCache.Set<string>("registerPostEvictionCallback", ($"key : {key} , reason : {reason}"));
            });
            
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(),options);

            Product product = new Product()
            {
                Id = 1,
                Name = "Kalem1",
                Price = 200
            };

            _memoryCache.Set<Product>("product:1", product);

            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("zaman", out string zamanCache);
            _memoryCache.TryGetValue("registerPostEvictionCallback", out string registerPostEvictionCallbackCache);
            _memoryCache.TryGetValue("product:1", out Product product);


            ViewBag.zaman = zamanCache;
            ViewBag.registerPostEvictionCallback = registerPostEvictionCallbackCache;
            ViewBag.product = product;

            return View();
        }
    }
}
