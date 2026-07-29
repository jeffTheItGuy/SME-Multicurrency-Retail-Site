using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class GoodsReceipt
{
    public int GrnId { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public int? PoId { get; set; }
    public int SupplierId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal Total { get; set; }
    public GrnStatus Status { get; set; } = GrnStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public PurchaseOrder? PurchaseOrder { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<GrnItem> GrnItems { get; set; } = new List<GrnItem>();
}
