using Meilisearch;
using SmeRetail.Models.Entities;
using SmeRetail.Data;
using Microsoft.EntityFrameworkCore;

namespace SmeRetail.Services;

public interface IMeilisearchService
{
    Task IndexProductAsync(Product product);
    Task IndexAllProductsAsync();
    Task<IReadOnlyCollection<Product>> SearchAsync(string query, int limit = 20);
}

public class MeilisearchService : IMeilisearchService
{
    private readonly MeilisearchClient _client;
    private readonly IServiceProvider _services;
    private readonly ILogger<MeilisearchService> _logger;
    private const string IndexName = "products";

    public MeilisearchService(IConfiguration config, IServiceProvider services, ILogger<MeilisearchService> logger)
    {
        var url = config["Meilisearch:Url"] ?? "http://localhost:7700";
        var key = config["Meilisearch:Key"] ?? "";
        _client = new MeilisearchClient(url, key);
        _services = services;
        _logger = logger;
    }

    public async Task IndexProductAsync(Product product)
    {
        try
        {
            var index = _client.Index(IndexName);
            var doc = new
            {
                id = product.ProductId,
                sku = product.Sku,
                barcode = product.Barcode,
                name = product.ProductName,
                description = product.Description,
                category = product.Category?.CategoryName,
                isActive = product.IsActive
            };
            await index.AddDocumentsAsync(new[] { doc });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to index product {ProductId}", product.ProductId);
        }
    }

    public async Task IndexAllProductsAsync()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var products = await db.Products
            .Include(p => p.Category)
            .ToListAsync();

        var index = _client.Index(IndexName);
        var docs = products.Select(p => new
        {
            id = p.ProductId,
            sku = p.Sku,
            barcode = p.Barcode,
            name = p.ProductName,
            description = p.Description,
            category = p.Category?.CategoryName,
            isActive = p.IsActive
        }).ToList();

        await index.AddDocumentsAsync(docs);
        _logger.LogInformation("Indexed {Count} products", docs.Count);
    }

    public async Task<IReadOnlyCollection<Product>> SearchAsync(string query, int limit = 20)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            var index = _client.Index(IndexName);
            var result = await index.SearchAsync<MeiliProduct>(query, new SearchQuery { Limit = limit });
            var ids = result.Hits.Select(h => h.id).ToList();
            if (!ids.Any()) return new List<Product>();

            return await db.Products
                .Include(p => p.Category)
                .Include(p => p.Prices)
                .Where(p => ids.Contains(p.ProductId))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Meilisearch query failed, falling back to SQL LIKE");
            return await db.Products
                .Include(p => p.Category)
                .Include(p => p.Prices)
                .Where(p => EF.Functions.Like(p.ProductName, $"%{query}%") 
                         || EF.Functions.Like(p.Sku, $"%{query}%")
                         || (p.Barcode != null && EF.Functions.Like(p.Barcode, $"%{query}%")))
                .Take(limit)
                .ToListAsync();
        }
    }

    private class MeiliProduct
    {
        public int id { get; set; }
        public string? sku { get; set; }
        public string? barcode { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? category { get; set; }
        public bool isActive { get; set; }
    }
}
