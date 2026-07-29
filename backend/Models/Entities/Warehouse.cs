namespace SmeRetail.Models.Entities;

public class Warehouse
{
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    
    // FIX: Added missing collection referenced by AppDbContext line 131
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}