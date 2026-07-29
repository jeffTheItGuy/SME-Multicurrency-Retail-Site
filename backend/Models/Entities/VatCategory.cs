namespace SmeRetail.Models.Entities;

public class VatCategory
{
    public int VatCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal VatRate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<VatRecord> VatRecords { get; set; } = new List<VatRecord>();
}
