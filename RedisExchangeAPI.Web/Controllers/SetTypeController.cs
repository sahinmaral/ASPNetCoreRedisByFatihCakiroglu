using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SetTypeController : Controller
    {
        private RedisService _redisService;
        private readonly IDatabase db;

        private string setKey = "colours";
        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {

            HashSet<string> colourSet = new HashSet<string>();
            if (db.KeyExistsAsync(setKey).Result)
            {
                db.SetMembersAsync(setKey).Result.ToList().ForEach(element =>
                {
                    colourSet.Add(element.ToString());
                });
            }


            return View(colourSet);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(string colour)
        {
            if(!db.KeyExistsAsync(setKey).Result)
                await db.KeyExpireAsync(setKey, DateTime.Now.AddSeconds(10));

            await db.SetAddAsync(setKey,colour);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItemAsync(string colour)
        {
            await db.SetRemoveAsync(setKey,colour);

            return RedirectToAction(nameof(Index));
        }
    }
}
