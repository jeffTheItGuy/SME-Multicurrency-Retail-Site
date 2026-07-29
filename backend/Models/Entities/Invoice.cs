using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class Invoice
{
    public int InvoiceId { get; set; }
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? ZimraInvoiceNumber { get; set; }
    public string? FiscalDeviceId { get; set; }
    public string? FiscalSignature { get; set; }
    public int CustomerId { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1;
    public decimal Subtotal { get; set; }
    public decimal VatTotal { get; set; }
    public decimal Total { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public bool SubmittedToZimra { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Sale Sale { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<VatRecord> VatRecords { get; set; } = new List<VatRecord>();
}
