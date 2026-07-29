namespace SmeRetail.Models.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int UomId { get; set; }
    public int VatCategoryId { get; set; }
    public bool TrackInventory { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ProductCategory Category { get; set; } = null!;
    public UnitOfMeasure Uom { get; set; } = null!;
    public VatCategory VatCategory { get; set; } = null!;
    public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    
    // FIX: Added missing collections referenced by AppDbContext lines 158 and 210
    public ICollection<GrnItem> GrnItems { get; set; } = new List<GrnItem>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}