namespace SmeRetail.Models.Entities;

public class SaleItem
{
    public int SaleItemId { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public decimal LineTotal { get; set; }

    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
