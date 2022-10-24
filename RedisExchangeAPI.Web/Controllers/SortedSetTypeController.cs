using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SortedSetTypeController : Controller
    {
        private RedisService _redisService;
        private readonly IDatabase db;

        private string sortedSetKey = "cars";
        public SortedSetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {

            SortedSet<string> carSet = new SortedSet<string>();
            if (db.KeyExistsAsync(sortedSetKey).Result)
            {
                db.SortedSetScan(sortedSetKey).ToList().ForEach(element =>
                {
                    carSet.Add(element.ToString());
                });
            }


            return View(carSet);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(string car,int score)
        {
            if(!db.KeyExistsAsync(sortedSetKey).Result)
                await db.KeyExpireAsync(sortedSetKey, DateTime.Now.AddSeconds(10));

            await db.SortedSetAddAsync(sortedSetKey, car,score);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItemAsync(string car)
        {
            await db.SortedSetRemoveAsync(sortedSetKey, car.Split(":")[0]);

            return RedirectToAction(nameof(Index));
        }
    }
}
