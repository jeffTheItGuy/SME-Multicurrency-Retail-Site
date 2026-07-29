using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeRetail.Data;
using SmeRetail.Models.Entities;
using SmeRetail.Dtos;
using SmeRetail.Services;

namespace SmeRetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMeilisearchService _search;

    public ProductsController(AppDbContext db, IMeilisearchService search)
    {
        _db = db;
        _search = search;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Include(p => p.VatCategory)
            .Include(p => p.Prices)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchResults = await _search.SearchAsync(search, pageSize);
            var ids = searchResults.Select(s => s.ProductId).ToList();
            query = query.Where(p => ids.Contains(p.ProductId));
        }

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(p => new ProductResponseDto(
            p.ProductId,
            p.Sku,
            p.Barcode,
            p.ProductName,
            p.Description,
            p.Category?.CategoryName ?? "",
            p.Uom?.UomCode ?? "",
            p.VatCategory?.VatRate ?? 0,
            p.TrackInventory,
            p.IsActive,
            p.Prices.Select(pr => new ProductPriceDto(pr.CurrencyCode, pr.PriceType.ToString(), pr.Price)).ToList()
        )).ToList();

        return Ok(new { total, page, pageSize, data = dtos });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Uom)
            .Include(p => p.VatCategory)
            .Include(p => p.Prices)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (p == null) return NotFound();

        return Ok(new ProductResponseDto(
            p.ProductId, p.Sku, p.Barcode, p.ProductName, p.Description,
            p.Category?.CategoryName ?? "", p.Uom?.UomCode ?? "", p.VatCategory?.VatRate ?? 0,
            p.TrackInventory, p.IsActive,
            p.Prices.Select(pr => new ProductPriceDto(pr.CurrencyCode, pr.PriceType.ToString(), pr.Price)).ToList()
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Sku = dto.Sku,
            Barcode = dto.Barcode,
            ProductName = dto.ProductName,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            UomId = dto.UomId,
            VatCategoryId = dto.VatCategoryId,
            TrackInventory = dto.TrackInventory
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        await _search.IndexProductAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.VatCategoryId = dto.VatCategoryId;
        product.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        await _search.IndexProductAsync(product);
        return NoContent();
    }

    [HttpPost("{id}/prices")]
    public async Task<IActionResult> SetPrice(int id, [FromBody] ProductPriceDto dto)
    {
        if (!Enum.TryParse<Models.Enums.PriceType>(dto.PriceType, true, out var priceType))
            return BadRequest("Invalid price type");

        var existing = await _db.ProductPrices
            .FirstOrDefaultAsync(p => p.ProductId == id && p.CurrencyCode == dto.CurrencyCode && p.PriceType == priceType);

        if (existing != null)
        {
            existing.Price = dto.Price;
            existing.EffectiveFrom = DateTime.UtcNow;
        }
        else
        {
            _db.ProductPrices.Add(new ProductPrice
            {
                ProductId = id,
                CurrencyCode = dto.CurrencyCode,
                PriceType = priceType,
                Price = dto.Price,
                EffectiveFrom = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> Reindex()
    {
        await _search.IndexAllProductsAsync();
        return Ok(new { message = "Reindexing started" });
    }
}
