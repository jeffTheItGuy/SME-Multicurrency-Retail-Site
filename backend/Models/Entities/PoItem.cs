namespace SmeRetail.Models.Entities;

public class PoItem
{
    public int PoItemId { get; set; }
    public int PoId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<GrnItem> GrnItems { get; set; } = new List<GrnItem>();
}
