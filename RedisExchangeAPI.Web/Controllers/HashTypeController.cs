using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class HashTypeController : BaseController
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;

        private string hashKey = "dictionary";
        public HashTypeController(RedisService redisService) : base(redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            Dictionary<string, string> dictionaries = new Dictionary<string, string>();

            if (db.KeyExistsAsync(hashKey).Result)
            {
                db.HashGetAllAsync(hashKey).Result.ToList().ForEach(element =>
                {
                    dictionaries.Add(element.Name, element.Value);
                });
            }


            return View(dictionaries);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(string key, string value)
        {
            await db.HashSetAsync(hashKey, key, value);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItemAsync(string key)
        {
            await db.HashDeleteAsync(hashKey, key);

            return RedirectToAction(nameof(Index));
        }
    }
}
