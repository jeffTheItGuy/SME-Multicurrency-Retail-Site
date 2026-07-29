using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class ProductPrice
{
    public int PriceId { get; set; }
    public int ProductId { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public PriceType PriceType { get; set; }
    public decimal Price { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
    public Currency Currency { get; set; } = null!;
}
