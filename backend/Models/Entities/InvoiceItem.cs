namespace SmeRetail.Models.Entities;

public class InvoiceItem
{
    public int InvoiceItemId { get; set; }
    public int InvoiceId { get; set; }
    public int SaleItemId { get; set; }
    public int ProductId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal LineTotal { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public SaleItem SaleItem { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
