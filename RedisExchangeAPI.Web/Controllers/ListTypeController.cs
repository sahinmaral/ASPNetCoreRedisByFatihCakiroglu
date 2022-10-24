using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private RedisService _redisService;
        private readonly IDatabase db;

        private string listKey = "names";
        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            List<string> names = new List<string>();

            if (db.KeyExistsAsync(listKey).Result)
            {
                db.ListRangeAsync(listKey).Result.ToList().ForEach(element =>
                {
                    names.Add(element.ToString());
                });

            }


            return View(names);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(string name)
        {
            await db.ListRightPushAsync(listKey, name);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItemAsync(string name)
        {
            await db.ListRemoveAsync(listKey, name);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteFirstItemAsync()
        {
            await db.ListLeftPopAsync(listKey);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteLastItemAsync()
        {
            await db.ListRightPopAsync(listKey);

            return RedirectToAction(nameof(Index));
        }
    }
}
