using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class Sale
{
    public int SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int WarehouseId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal Subtotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal Total { get; set; }
    public SaleStatus PaymentStatus { get; set; } = SaleStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public Invoice? Invoice { get; set; }
}
