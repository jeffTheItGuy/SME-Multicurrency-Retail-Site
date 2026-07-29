namespace SmeRetail.Models.Entities;

public class Inventory
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public decimal Quantity { get; set; } = 0;
    public decimal AverageCost { get; set; } = 0;
    public string CostCurrency { get; set; } = "USD";
    public decimal ReorderLevel { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
