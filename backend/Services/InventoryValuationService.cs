using SmeRetail.Models.Entities;
using SmeRetail.Models.Enums;
using SmeRetail.Data;
using Microsoft.EntityFrameworkCore;

namespace SmeRetail.Services;

public interface IInventoryValuationService
{
    Task ReceiveStockAsync(int productId, int warehouseId, decimal quantity, decimal unitCost, string currencyCode);
    Task<decimal?> ConsumeStockAsync(int productId, int warehouseId, decimal quantity);
    Task AdjustStockAsync(int productId, int warehouseId, decimal quantityDelta, string reason);
}

public class InventoryValuationService : IInventoryValuationService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InventoryValuationService> _logger;

    public InventoryValuationService(AppDbContext db, ILogger<InventoryValuationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ReceiveStockAsync(int productId, int warehouseId, decimal quantity, decimal unitCost, string currencyCode)
    {
        var inv = await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);

        if (inv == null)
        {
            inv = new Inventory
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = quantity,
                AverageCost = unitCost,
                CostCurrency = currencyCode,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Inventories.Add(inv);
        }
        else
        {
            // Weighted average cost update
            var oldQty = inv.Quantity;
            var oldCost = inv.AverageCost;
            var newQty = oldQty + quantity;

            if (newQty > 0)
            {
                inv.AverageCost = ((oldQty * oldCost) + (quantity * unitCost)) / newQty;
            }
            inv.Quantity = newQty;
            inv.CostCurrency = currencyCode;
            inv.UpdatedAt = DateTime.UtcNow;
        }

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            TransactionType = TransactionType.Purchase,
            Quantity = quantity,
            UnitCost = unitCost,
            TransactionDate = DateTime.UtcNow,
            Notes = $"Stock received at {unitCost} {currencyCode}"
        });

        await _db.SaveChangesAsync();
        _logger.LogInformation("Received {Qty} of product {ProductId} into warehouse {WarehouseId}", quantity, productId, warehouseId);
    }

    public async Task<decimal?> ConsumeStockAsync(int productId, int warehouseId, decimal quantity)
    {
        var inv = await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);

        if (inv == null || inv.Quantity < quantity)
        {
            _logger.LogWarning("Insufficient stock for product {ProductId} in warehouse {WarehouseId}", productId, warehouseId);
            return null;
        }

        var costAtSale = inv.AverageCost;
        inv.Quantity -= quantity;
        inv.UpdatedAt = DateTime.UtcNow;

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            TransactionType = TransactionType.Sale,
            Quantity = -quantity,
            UnitCost = costAtSale,
            TransactionDate = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return costAtSale;
    }

    public async Task AdjustStockAsync(int productId, int warehouseId, decimal quantityDelta, string reason)
    {
        var inv = await _db.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && i.WarehouseId == warehouseId);

        if (inv == null && quantityDelta > 0)
        {
            inv = new Inventory
            {
                ProductId = productId,
                WarehouseId = warehouseId,
                Quantity = quantityDelta,
                AverageCost = 0,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Inventories.Add(inv);
        }
        else if (inv != null)
        {
            inv.Quantity += quantityDelta;
            inv.UpdatedAt = DateTime.UtcNow;
        }

        _db.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            TransactionType = TransactionType.Adjustment,
            Quantity = quantityDelta,
            TransactionDate = DateTime.UtcNow,
            Notes = reason
        });

        await _db.SaveChangesAsync();
    }
}
