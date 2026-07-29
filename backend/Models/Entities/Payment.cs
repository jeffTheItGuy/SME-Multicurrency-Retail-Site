using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int SaleId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal Amount { get; set; }
    public decimal ExchangeRate { get; set; } = 1;
    public decimal BaseCurrencyAmount { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public Sale Sale { get; set; } = null!;
}
