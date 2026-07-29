using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeRetail.Data;
using SmeRetail.Dtos;
using SmeRetail.Services;

namespace SmeRetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInventoryValuationService _inventory;

    public InventoryController(AppDbContext db, IInventoryValuationService inventory)
    {
        _db = db;
        _inventory = inventory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? warehouseId, [FromQuery] int? productId)
    {
        var query = _db.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .AsNoTracking();

        if (warehouseId.HasValue) query = query.Where(i => i.WarehouseId == warehouseId.Value);
        if (productId.HasValue) query = query.Where(i => i.ProductId == productId.Value);

        var items = await query.ToListAsync();
        var dtos = items.Select(i => new InventoryResponseDto(
            i.InventoryId, i.ProductId, i.Product?.ProductName ?? "", i.WarehouseId,
            i.Warehouse?.WarehouseName ?? "", i.Quantity, i.AverageCost, i.CostCurrency, i.ReorderLevel
        )).ToList();

        return Ok(dtos);
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int? productId, [FromQuery] int? warehouseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _db.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .AsNoTracking();

        if (productId.HasValue) query = query.Where(t => t.ProductId == productId.Value);
        if (warehouseId.HasValue) query = query.Where(t => t.WarehouseId == warehouseId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = items });
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust([FromBody] StockAdjustmentDto dto)
    {
        await _inventory.AdjustStockAsync(dto.ProductId, dto.WarehouseId, dto.QuantityDelta, dto.Reason);
        return Ok(new { message = "Stock adjusted" });
    }
}
