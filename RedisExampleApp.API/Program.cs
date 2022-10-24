using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;
using RedisExampleApp.API.Repositories;
using RedisExampleApp.API.Services;
using RedisExampleApp.Cache;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<RedisService>(serviceProvider =>
{
    return new RedisService(builder.Configuration["Caching:Options:Url"]);
});

builder.Services.AddSingleton<IDatabase>(serviceProvider =>
{
    var redisService = serviceProvider.GetRequiredService<RedisService>();
    return redisService.GetDb(0);
});

builder.Services.AddScoped<IProductRepository>(serviceProvider =>
{
    var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
    var productRepository = new ProductRepository(appDbContext);

    var redisService = serviceProvider.GetRequiredService<RedisService>();

    return new ProductRepositoryWithCacheDecorator(productRepository, redisService);
});

builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("redisDb");
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // For using InMemory Database , we have to initialize our database
    // everytime program initializes

    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
