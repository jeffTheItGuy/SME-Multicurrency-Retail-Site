using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeRetail.Data;
using SmeRetail.Models.Entities;
using SmeRetail.Models.Enums;
using SmeRetail.Dtos;
using SmeRetail.Services;

namespace SmeRetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcurementController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInventoryValuationService _inventory;

    public ProcurementController(AppDbContext db, IInventoryValuationService inventory)
    {
        _db = db;
        _inventory = inventory;
    }

    [HttpGet("purchase-orders")]
    public async Task<IActionResult> GetPurchaseOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _db.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Warehouse)
            .Include(p => p.PoItems).ThenInclude(i => i.Product)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = items });
    }

    [HttpPost("purchase-orders")]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto dto)
    {
        var poNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        var po = new PurchaseOrder
        {
            PoNumber = poNumber,
            SupplierId = dto.SupplierId,
            WarehouseId = dto.WarehouseId,
            CurrencyCode = dto.CurrencyCode,
            OrderDate = DateTime.UtcNow
        };

        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync();

        decimal total = 0;
        foreach (var item in dto.Items)
        {
            var lineTotal = item.Quantity * item.UnitPrice;
            total += lineTotal;
            _db.PoItems.Add(new PoItem
            {
                PoId = po.PoId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = lineTotal
            });
        }

        po.Total = total;
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPurchaseOrders), new { }, po);
    }

    [HttpGet("goods-receipts")]
    public async Task<IActionResult> GetGoodsReceipts([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _db.GoodsReceipts
            .Include(g => g.Supplier)
            .Include(g => g.Warehouse)
            .Include(g => g.GrnItems).ThenInclude(i => i.Product)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(g => g.ReceiptDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = items });
    }

    [HttpPost("goods-receipts")]
    public async Task<IActionResult> CreateGoodsReceipt([FromBody] CreateGrnDto dto)
    {
        var grn = new GoodsReceipt
        {
            GrnNumber = dto.GrnNumber,
            PoId = dto.PoId,
            SupplierId = dto.SupplierId,
            WarehouseId = dto.WarehouseId,
            CurrencyCode = dto.CurrencyCode,
            ReceiptDate = DateTime.UtcNow,
            Status = GrnStatus.Pending
        };

        _db.GoodsReceipts.Add(grn);
        await _db.SaveChangesAsync();

        decimal total = 0;
        foreach (var item in dto.Items)
        {
            var lineTotal = item.Quantity * item.UnitCost;
            total += lineTotal;
            _db.GrnItems.Add(new GrnItem
            {
                GrnId = grn.GrnId,
                PoItemId = item.PoItemId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                LineTotal = lineTotal
            });
        }

        grn.Total = total;
        await _db.SaveChangesAsync();
        return Ok(grn);
    }

    [HttpPost("goods-receipts/{id}/post")]
    public async Task<IActionResult> PostGrn(int id)
    {
        var grn = await _db.GoodsReceipts
            .Include(g => g.GrnItems)
            .FirstOrDefaultAsync(g => g.GrnId == id);

        if (grn == null) return NotFound();
        if (grn.Status == GrnStatus.Posted) return BadRequest(new { message = "Already posted" });

        foreach (var item in grn.GrnItems)
        {
            await _inventory.ReceiveStockAsync(item.ProductId, grn.WarehouseId, item.Quantity, item.UnitCost, grn.CurrencyCode);
        }

        grn.Status = GrnStatus.Posted;
        await _db.SaveChangesAsync();
        return Ok(new { message = "GRN posted and inventory updated" });
    }
}
