using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class PurchaseOrder
{
    public int PoId { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime OrderDate { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal Total { get; set; }
    public PoStatus Status { get; set; } = PoStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Supplier Supplier { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<PoItem> PoItems { get; set; } = new List<PoItem>();
    public ICollection<GoodsReceipt> GoodsReceipts { get; set; } = new List<GoodsReceipt>();
}
