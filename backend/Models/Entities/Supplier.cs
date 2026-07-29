namespace SmeRetail.Models.Entities;

public class Supplier
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; } = true;

    public Currency Currency { get; set; } = null!;
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
