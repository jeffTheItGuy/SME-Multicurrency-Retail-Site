using Microsoft.EntityFrameworkCore;
using SmeRetail.Models.Entities;
using SmeRetail.Models.Enums;

namespace SmeRetail.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<VatCategory> VatCategories => Set<VatCategory>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPrice> ProductPrices => Set<ProductPrice>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PoItem> PoItems => Set<PoItem>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GrnItem> GrnItems => Set<GrnItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<VatRecord> VatRecords => Set<VatRecord>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Currency
        modelBuilder.Entity<Currency>(e =>
        {
            e.HasKey(c => c.CurrencyCode);
            e.Property(c => c.CurrencyCode).HasMaxLength(3);
            e.Property(c => c.CurrencyName).HasMaxLength(100);
            e.Property(c => c.Symbol).HasMaxLength(10);
        });

        // Exchange Rate
        modelBuilder.Entity<ExchangeRate>(e =>
        {
            e.HasKey(r => r.RateId);
            e.HasIndex(r => new { r.FromCurrencyCode, r.ToCurrencyCode, r.RateType, r.EffectiveDate }).IsUnique();
            e.HasOne(r => r.FromCurrency).WithMany(c => c.FromRates).HasForeignKey(r => r.FromCurrencyCode);
            e.HasOne(r => r.ToCurrency).WithMany(c => c.ToRates).HasForeignKey(r => r.ToCurrencyCode);
        });

        // Product Category
        modelBuilder.Entity<ProductCategory>(e =>
        {
            e.HasKey(c => c.CategoryId);
            e.HasOne(c => c.ParentCategory).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentCategoryId);
        });

        // Unit of Measure
        modelBuilder.Entity<UnitOfMeasure>(e =>
        {
            e.HasKey(u => u.UomId);
            e.HasIndex(u => u.UomCode).IsUnique();
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.ProductId);
            e.HasIndex(p => p.Sku).IsUnique();
            e.HasIndex(p => p.Barcode).IsUnique();
            e.HasIndex(p => p.CategoryId);
            e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            e.HasOne(p => p.Uom).WithMany(u => u.Products).HasForeignKey(p => p.UomId);
            e.HasOne(p => p.VatCategory).WithMany(v => v.Products).HasForeignKey(p => p.VatCategoryId);
        });

        // Product Price
        modelBuilder.Entity<ProductPrice>(e =>
        {
            e.HasKey(p => p.PriceId);
            e.HasIndex(p => new { p.ProductId, p.CurrencyCode, p.PriceType }).IsUnique();
            e.HasOne(p => p.Product).WithMany(pr => pr.Prices).HasForeignKey(p => p.ProductId);
            e.HasOne(p => p.Currency).WithMany(c => c.ProductPrices).HasForeignKey(p => p.CurrencyCode);
        });

        // Warehouse
        modelBuilder.Entity<Warehouse>(e =>
        {
            e.HasKey(w => w.WarehouseId);
            e.HasIndex(w => w.WarehouseCode).IsUnique();
        });

        // Inventory
        modelBuilder.Entity<Inventory>(e =>
        {
            e.HasKey(i => i.InventoryId);
            e.HasIndex(i => new { i.ProductId, i.WarehouseId }).IsUnique();
            e.HasOne(i => i.Product).WithMany(p => p.Inventories).HasForeignKey(i => i.ProductId);
            e.HasOne(i => i.Warehouse).WithMany(w => w.Inventories).HasForeignKey(i => i.WarehouseId);
        });

        // Inventory Transaction
        modelBuilder.Entity<InventoryTransaction>(e =>
        {
            e.HasKey(t => t.TransactionId);
            e.HasIndex(t => new { t.ProductId, t.WarehouseId, t.TransactionDate });
            e.HasOne(t => t.Product).WithMany().HasForeignKey(t => t.ProductId);
            e.HasOne(t => t.Warehouse).WithMany().HasForeignKey(t => t.WarehouseId);
        });

        // Supplier
        modelBuilder.Entity<Supplier>(e =>
        {
            e.HasKey(s => s.SupplierId);
            e.HasOne(s => s.Currency).WithMany().HasForeignKey(s => s.CurrencyCode);
        });

        // Purchase Order
        modelBuilder.Entity<PurchaseOrder>(e =>
        {
            e.HasKey(p => p.PoId);
            e.HasIndex(p => p.PoNumber).IsUnique();
            e.HasOne(p => p.Supplier).WithMany(s => s.PurchaseOrders).HasForeignKey(p => p.SupplierId);
            e.HasOne(p => p.Warehouse).WithMany(w => w.PurchaseOrders).HasForeignKey(p => p.WarehouseId);
        });

        // PO Item
        modelBuilder.Entity<PoItem>(e =>
        {
            e.HasKey(i => i.PoItemId);
            e.HasOne(i => i.PurchaseOrder).WithMany(po => po.PoItems).HasForeignKey(i => i.PoId);
            e.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId);
        });

        // Goods Receipt
        modelBuilder.Entity<GoodsReceipt>(e =>
        {
            e.HasKey(g => g.GrnId);
            e.HasIndex(g => g.GrnNumber).IsUnique();
            e.HasOne(g => g.PurchaseOrder).WithMany(po => po.GoodsReceipts).HasForeignKey(g => g.PoId);
            e.HasOne(g => g.Supplier).WithMany().HasForeignKey(g => g.SupplierId);
            e.HasOne(g => g.Warehouse).WithMany().HasForeignKey(g => g.WarehouseId);
        });

        // GRN Item
        modelBuilder.Entity<GrnItem>(e =>
        {
            e.HasKey(i => i.GrnItemId);
            e.HasOne(i => i.GoodsReceipt).WithMany(g => g.GrnItems).HasForeignKey(i => i.GrnId);
            e.HasOne(i => i.PoItem).WithMany(p => p.GrnItems).HasForeignKey(i => i.PoItemId);
            e.HasOne(i => i.Product).WithMany(p => p.GrnItems).HasForeignKey(i => i.ProductId);
        });

        // Customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.CustomerId);
            e.HasOne(c => c.Currency).WithMany().HasForeignKey(c => c.CurrencyCode);
        });

        // Sale
        modelBuilder.Entity<Sale>(e =>
        {
            e.HasKey(s => s.SaleId);
            e.HasIndex(s => s.SaleNumber).IsUnique();
            e.HasIndex(s => new { s.CustomerId, s.SaleDate });
            e.HasOne(s => s.Customer).WithMany(c => c.Sales).HasForeignKey(s => s.CustomerId);
            e.HasOne(s => s.Warehouse).WithMany(w => w.Sales).HasForeignKey(s => s.WarehouseId);
        });

        // Sale Item
        modelBuilder.Entity<SaleItem>(e =>
        {
            e.HasKey(i => i.SaleItemId);
            e.HasIndex(i => new { i.SaleId, i.ProductId });
            e.HasOne(i => i.Sale).WithMany(s => s.SaleItems).HasForeignKey(i => i.SaleId);
            e.HasOne(i => i.Product).WithMany(p => p.SaleItems).HasForeignKey(i => i.ProductId);
        });

        // Payment
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(p => p.PaymentId);
            e.HasOne(p => p.Sale).WithMany(s => s.Payments).HasForeignKey(p => p.SaleId);
        });

        // Invoice
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(i => i.InvoiceId);
            e.HasIndex(i => i.InvoiceNumber).IsUnique();
            e.HasIndex(i => i.FiscalInvoiceNumber).IsUnique();
            e.HasOne(i => i.Sale).WithOne(s => s.Invoice).HasForeignKey<Invoice>(i => i.SaleId);
            e.HasOne(i => i.Customer).WithMany().HasForeignKey(i => i.CustomerId);
        });

        // Invoice Item
        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.HasKey(i => i.InvoiceItemId);
            e.HasOne(i => i.Invoice).WithMany(inv => inv.InvoiceItems).HasForeignKey(i => i.InvoiceId);
            e.HasOne(i => i.SaleItem).WithMany(si => si.InvoiceItems).HasForeignKey(i => i.SaleItemId);
            e.HasOne(i => i.Product).WithMany(p => p.InvoiceItems).HasForeignKey(i => i.ProductId);
        });

        // VAT Record
        modelBuilder.Entity<VatRecord>(e =>
        {
            e.HasKey(v => v.VatRecordId);
            e.HasIndex(v => new { v.TaxPeriod, v.SubmittedToAuthority });
            e.HasOne(v => v.Invoice).WithMany(i => i.VatRecords).HasForeignKey(v => v.InvoiceId);
            e.HasOne(v => v.VatCategory).WithMany(vc => vc.VatRecords).HasForeignKey(v => v.VatCategoryId);
        });

        // Stock Adjustment
        modelBuilder.Entity<StockAdjustment>(e =>
        {
            e.HasKey(s => s.AdjustmentId);
            e.HasOne(s => s.Warehouse).WithMany().HasForeignKey(s => s.WarehouseId);
        });

        // Stock Transfer
        modelBuilder.Entity<StockTransfer>(e =>
        {
            e.HasKey(s => s.TransferId);
            e.HasOne(s => s.FromWarehouse).WithMany().HasForeignKey(s => s.FromWarehouseId);
            e.HasOne(s => s.ToWarehouse).WithMany().HasForeignKey(s => s.ToWarehouseId);
        });
    }
}

