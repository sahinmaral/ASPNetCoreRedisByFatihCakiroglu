using IDistributeCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace IDistributeCacheRedisApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> IndexAsync()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(10);


            Product product = new Product()
            {
                Name = "Kalem", Price = 30
            };

            Product product2 = new Product()
            {
                Name = "Silgi", Price = 10
            };

            string serializedProduct = JsonConvert.SerializeObject(product);

            Byte[] byteProductSilgi = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(product2));

            await _distributedCache.SetStringAsync("time", DateTime.Now.ToString(),options);
            await _distributedCache.SetStringAsync("product:Kalem", serializedProduct,options);
            await _distributedCache.SetAsync("product:Silgi", byteProductSilgi,options);

            return View();
        }

        public async Task<IActionResult> ShowAsync()
        {
            ViewBag.Time = await _distributedCache.GetStringAsync("time");

            string serializedProductKalem = await _distributedCache.GetStringAsync("product:Kalem");
            ViewBag.Product1 = JsonConvert.DeserializeObject<Product>(serializedProductKalem);

            Byte[] byteProductSilgi = await _distributedCache.GetAsync("product:Silgi");
            ViewBag.Product2 = JsonConvert.DeserializeObject<Product>(Encoding.UTF8.GetString(byteProductSilgi));

            return View();
        }

        public async Task<IActionResult> RemoveAsync()
        {
            await _distributedCache.RemoveAsync("time");
            await _distributedCache.RemoveAsync("product:Kalem");

            return View();
        }

        public async Task<IActionResult> CacheImageAsync()
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(10);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/messi.jpg");

            byte[] imageByte = await System.IO.File.ReadAllBytesAsync(path);

            await _distributedCache.SetAsync("messi-image",imageByte,options);

            return View();
        }

        public async Task<IActionResult> GetImageAsync()
        {
            byte[] imageByte = await _distributedCache.GetAsync("messi-image");

            return File(imageByte, "image/jpg");
        }
    }
}
