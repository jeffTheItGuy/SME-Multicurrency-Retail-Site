namespace SmeRetail.Models.Entities;

public class GrnItem
{
    public int GrnItemId { get; set; }
    public int GrnId { get; set; }
    public int? PoItemId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal { get; set; }

    public GoodsReceipt GoodsReceipt { get; set; } = null!;
    public PoItem? PoItem { get; set; }
    public Product Product { get; set; } = null!;
}
