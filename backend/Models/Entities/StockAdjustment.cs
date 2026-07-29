namespace SmeRetail.Models.Entities;

public class StockAdjustment
{
    public int AdjustmentId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime AdjustmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal TotalValueImpact { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Warehouse Warehouse { get; set; } = null!;
}
