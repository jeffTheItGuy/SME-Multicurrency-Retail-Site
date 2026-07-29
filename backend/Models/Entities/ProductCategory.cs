namespace SmeRetail.Models.Entities;

public class ProductCategory
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public ProductCategory? ParentCategory { get; set; }
    public ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
