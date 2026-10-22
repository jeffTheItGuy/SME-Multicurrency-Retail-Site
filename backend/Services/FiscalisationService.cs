using SmeRetail.Models.Entities;
using SmeRetail.Data;
using Microsoft.EntityFrameworkCore;

namespace SmeRetail.Services;

public interface IFiscalisationService
{
    Task<Invoice> FiscaliseAsync(int saleId);
    Task<List<Invoice>> GetPendingSubmissionsAsync();
    Task MarkAsSubmittedAsync(int invoiceId, string fiscalInvoiceNumber, string ackCode);
}

public class FiscalisationService : IFiscalisationService
{
    private readonly AppDbContext _db;
    private readonly ILogger<FiscalisationService> _logger;

    public FiscalisationService(AppDbContext db, ILogger<FiscalisationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Invoice> FiscaliseAsync(int saleId)
    {
        var sale = await _db.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .ThenInclude(p => p.VatCategory)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);

        if (sale == null) throw new ArgumentException("Sale not found");
        if (sale.Invoice != null) return sale.Invoice;

        var invoiceNumber = $"INV-{sale.SaleNumber}";
        var invoice = new Invoice
        {
            SaleId = sale.SaleId,
            InvoiceNumber = invoiceNumber,
            CustomerId = sale.CustomerId ?? 0,
            CurrencyCode = sale.CurrencyCode,
            ExchangeRate = sale.ExchangeRate,
            Subtotal = sale.Subtotal,
            VatTotal = sale.VatTotal,
            Total = sale.Total,
            Status = Models.Enums.InvoiceStatus.Draft,
            SubmittedToAuthority = false
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        foreach (var si in sale.SaleItems)
        {
            var ii = new InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                SaleItemId = si.SaleItemId,
                ProductId = si.ProductId,
                Description = si.Product.ProductName,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                VatRate = si.VatRate,
                VatAmount = si.VatAmount,
                LineTotal = si.LineTotal
            };
            _db.InvoiceItems.Add(ii);

            if (si.Product.VatCategory != null)
            {
                _db.VatRecords.Add(new VatRecord
                {
                    InvoiceId = invoice.InvoiceId,
                    VatCategoryId = si.Product.VatCategoryId,
                    TaxableAmount = si.UnitPrice * si.Quantity,
                    VatRate = si.VatRate,
                    VatAmount = si.VatAmount,
                    TaxPeriod = DateTime.UtcNow.ToString("yyyy-MM"),
                    SubmittedToAuthority = false
                });
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Invoice {InvoiceNumber} created for sale {SaleId}", invoiceNumber, saleId);
        return invoice;
    }

    public async Task<List<Invoice>> GetPendingSubmissionsAsync()
    {
        return await _db.Invoices
            .Where(i => !i.SubmittedToAuthority && i.Status == Models.Enums.InvoiceStatus.Fiscalised)
            .Include(i => i.VatRecords)
            .ToListAsync();
    }

    public async Task MarkAsSubmittedAsync(int invoiceId, string fiscalInvoiceNumber, string ackCode)
    {
        var invoice = await _db.Invoices
            .Include(i => i.VatRecords)
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

        if (invoice == null) return;

        invoice.FiscalInvoiceNumber = fiscalInvoiceNumber;
        invoice.SubmittedToAuthority = true;
        invoice.Status = Models.Enums.InvoiceStatus.Fiscalised;

        foreach (var vr in invoice.VatRecords)
        {
            vr.SubmittedToAuthority = true;
            vr.AuthorityAckCode = ackCode;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Invoice {InvoiceId} marked as submitted", invoiceId);
    }
}
