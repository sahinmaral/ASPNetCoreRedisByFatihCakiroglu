using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class BaseController : Controller
    {
        protected RedisService _redisService;
        protected readonly IDatabase db;
        public BaseController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
    }
}
