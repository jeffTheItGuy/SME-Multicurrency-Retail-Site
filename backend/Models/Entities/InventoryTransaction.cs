using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class InventoryTransaction
{
    public int TransactionId { get; set; }
    public TransactionType TransactionType { get; set; }
    public string ReferenceType { get; set; } = string.Empty; // SALE, PO, GRN, ADJUSTMENT, TRANSFER
    public int ReferenceId { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
