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
}
