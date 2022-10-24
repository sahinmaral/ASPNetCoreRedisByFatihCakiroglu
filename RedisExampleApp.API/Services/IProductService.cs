using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Services
{
    public interface IProductService
    {
        public Task<Product> GetByIdAsync(int id);
        public Task<List<Product>> GetAsync();
        Task<Product> AddAsync(Product product);
    }
}
