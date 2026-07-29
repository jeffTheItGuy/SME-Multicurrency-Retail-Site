namespace SmeRetail.Models.Entities;

public class UnitOfMeasure
{
    public int UomId { get; set; }
    public string UomCode { get; set; } = string.Empty;
    public string UomName { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
