using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Repositories
{
    public interface IProductRepository
    {
        public Task<Product> GetByIdAsync(int id);
        public Task<List<Product>> GetAsync();
        Task<Product> AddAsync(Product product);
    }
}
