using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;

namespace RedisExampleApp.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task<Product> AddAsync(Product product)
        {
            return await _repository.AddAsync(product);
        }

        public async Task<List<Product>> GetAsync()
        {
            return await _repository.GetAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
