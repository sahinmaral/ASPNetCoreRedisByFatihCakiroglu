
using RedisExampleApp.API.Models;
using RedisExampleApp.Cache;
using StackExchange.Redis;
using System.Linq;
using System.Text.Json;

namespace RedisExampleApp.API.Repositories
{
    public class ProductRepositoryWithCacheDecorator : IProductRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepository;

        private const string HASH_KEY = "productCaches";
        public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService)
        {
            _productRepository = productRepository;
            _redisService = redisService;
            _cacheRepository = _redisService.GetDb(0);
        }
        public async Task<Product> AddAsync(Product product)
        {
            var newProduct = await _productRepository.AddAsync(product);

            if (_cacheRepository.KeyExistsAsync(HASH_KEY).Result)
            {
                await _cacheRepository.HashSetAsync(HASH_KEY, newProduct.Id, JsonSerializer.Serialize(newProduct));
            }

            return newProduct;
        }

        public async Task<List<Product>> GetAsync()
        {
            if (!_cacheRepository.KeyExistsAsync(HASH_KEY).Result)
                return await LoadToCacheFromDbAsync();

            var products = new List<Product>();
            foreach (var hashEntry in _cacheRepository.HashGetAllAsync(HASH_KEY).Result.ToList())
            {
                var product = JsonSerializer.Deserialize<Product>(hashEntry.Value);
                products.Add(product);
            }

            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (_cacheRepository.KeyExistsAsync(HASH_KEY).Result)
            {
                var product = await _cacheRepository.HashGetAsync(HASH_KEY, id);
                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
            }

            var products = await LoadToCacheFromDbAsync();
            return products.FirstOrDefault(x=> x.Id == id);

        }

        private async Task<List<Product>> LoadToCacheFromDbAsync()
        {
            var products = await _productRepository.GetAsync();

            products.ForEach(product =>
            {
                _cacheRepository.HashSetAsync(HASH_KEY, product.Id, JsonSerializer.Serialize(product));
            });

            return products;
        }
    }
}
