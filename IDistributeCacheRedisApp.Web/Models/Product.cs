namespace IDistributeCacheRedisApp.Web.Models
{
    public class Product
    {
        public Product()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
