namespace SmeRetail.Models.Entities;

public class StockTransfer
{
    public int TransferId { get; set; }
    public int FromWarehouseId { get; set; }
    public int ToWarehouseId { get; set; }
    public DateTime TransferDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InTransit, Completed, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Warehouse FromWarehouse { get; set; } = null!;
    public Warehouse ToWarehouse { get; set; } = null!;
}
