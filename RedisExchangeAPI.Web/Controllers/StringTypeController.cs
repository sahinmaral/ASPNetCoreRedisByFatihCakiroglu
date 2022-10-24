using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public async Task<IActionResult> IndexAsync()
        {

            await db.StringSetAsync("name","sahin maral");
            await db.StringSetAsync("ziyaretci", 100);

            return View();
        }

        public async Task<IActionResult> ShowAsync()
        {

            RedisValue redisValueNameSplit = await db.StringGetRangeAsync("name", 0, -1);

            await db.StringIncrementAsync("ziyaretci",10);
            long ziyaretci = db.StringDecrementAsync("ziyaretci",10).Result;

            RedisValue redisValueName = await db.StringGetAsync("name");
            RedisValue redisValueZiyaretci = await db.StringGetAsync("ziyaretci");


            ViewBag.name = redisValueName.HasValue ? redisValueName.ToString() : string.Empty;
            ViewBag.nameSplit = redisValueNameSplit.HasValue ? redisValueNameSplit.ToString() : string.Empty;

            ViewBag.ziyaretci = redisValueZiyaretci.HasValue ? redisValueZiyaretci.ToString() : string.Empty;

            return View();
        }
    }
}
