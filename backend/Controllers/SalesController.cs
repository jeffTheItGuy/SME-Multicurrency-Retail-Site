using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmeRetail.Data;
using SmeRetail.Models.Entities;
using SmeRetail.Models.Enums;
using SmeRetail.Dtos;
using SmeRetail.Services;

namespace SmeRetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IVatCalculator _vat;
    private readonly IInventoryValuationService _inventory;
    private readonly IExchangeRateService _exchange;

    public SalesController(AppDbContext db, IVatCalculator vat, IInventoryValuationService inventory, IExchangeRateService exchange)
    {
        _db = db;
        _vat = vat;
        _inventory = inventory;
        _exchange = exchange;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var query = _db.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .Include(s => s.Payments)
            .AsNoTracking();

        if (from.HasValue) query = query.Where(s => s.SaleDate >= from.Value);
        if (to.HasValue) query = query.Where(s => s.SaleDate <= to.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.SaleDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, data = items.Select(MapToDto) });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _db.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .Include(s => s.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SaleId == id);

        if (sale == null) return NotFound();
        return Ok(MapToDto(sale));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        var saleNumber = $"SL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        var sale = new Sale
        {
            SaleNumber = saleNumber,
            CustomerId = dto.CustomerId,
            WarehouseId = dto.WarehouseId,
            CurrencyCode = dto.CurrencyCode,
            ExchangeRate = await _exchange.GetRateAsync(dto.CurrencyCode, "USD"),
            SaleDate = DateTime.UtcNow
        };

        _db.Sales.Add(sale);
        await _db.SaveChangesAsync(); // Get SaleId

        decimal subtotal = 0, vatTotal = 0, discountTotal = 0;

        foreach (var item in dto.Items)
        {
            var product = await _db.Products
                .Include(p => p.VatCategory)
                .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

            if (product == null) continue;

            var (vatAmt, lineTotal) = await _vat.CalculateVatAsync(item.UnitPrice, item.Quantity, product.VatCategory?.VatRate ?? 0);
            var discount = item.DiscountAmount ?? 0;
            lineTotal -= discount;

            decimal? unitCost = null;
            if (product.TrackInventory)
            {
                unitCost = await _inventory.ConsumeStockAsync(item.ProductId, dto.WarehouseId, item.Quantity);
                if (unitCost == null)
                    return BadRequest(new { message = $"Insufficient stock for product {product.ProductName}" });
            }

            var saleItem = new SaleItem
            {
                SaleId = sale.SaleId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                UnitCost = unitCost,
                VatRate = product.VatCategory?.VatRate ?? 0,
                VatAmount = vatAmt,
                LineTotal = lineTotal
            };

            _db.SaleItems.Add(saleItem);
            subtotal += (item.UnitPrice * item.Quantity) - discount;
            vatTotal += vatAmt;
            discountTotal += discount;
        }

        sale.Subtotal = subtotal;
        sale.VatTotal = vatTotal;
        sale.DiscountTotal = discountTotal;
        sale.Total = subtotal + vatTotal;

        // Process payments
        decimal paid = 0;
        if (dto.Payments != null)
        {
            foreach (var p in dto.Payments)
            {
                var baseAmt = p.CurrencyCode == "USD" ? p.Amount : await _exchange.ConvertAsync(p.Amount, p.CurrencyCode, "USD");
                _db.Payments.Add(new Payment
                {
                    SaleId = sale.SaleId,
                    PaymentMethod = p.PaymentMethod,
                    CurrencyCode = p.CurrencyCode,
                    Amount = p.Amount,
                    ExchangeRate = await _exchange.GetRateAsync(p.CurrencyCode, "USD"),
                    BaseCurrencyAmount = baseAmt,
                    ReferenceNumber = p.ReferenceNumber,
                    PaymentDate = DateTime.UtcNow
                });
                paid += p.Amount;
            }
        }

        sale.PaymentStatus = paid >= sale.Total ? SaleStatus.Paid : SaleStatus.Pending;
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sale.SaleId }, MapToDto(sale));
    }

    [HttpPost("{id}/payments")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] CreatePaymentDto dto)
    {
        var sale = await _db.Sales.Include(s => s.Payments).FirstOrDefaultAsync(s => s.SaleId == id);
        if (sale == null) return NotFound();

        var baseAmt = dto.CurrencyCode == "USD" ? dto.Amount : await _exchange.ConvertAsync(dto.Amount, dto.CurrencyCode, "USD");
        _db.Payments.Add(new Payment
        {
            SaleId = id,
            PaymentMethod = dto.PaymentMethod,
            CurrencyCode = dto.CurrencyCode,
            Amount = dto.Amount,
            ExchangeRate = await _exchange.GetRateAsync(dto.CurrencyCode, "USD"),
            BaseCurrencyAmount = baseAmt,
            ReferenceNumber = dto.ReferenceNumber,
            PaymentDate = DateTime.UtcNow
        });

        var totalPaid = sale.Payments.Sum(p => p.Amount) + dto.Amount;
        if (totalPaid >= sale.Total) sale.PaymentStatus = SaleStatus.Paid;

        await _db.SaveChangesAsync();
        return Ok(MapToDto(sale));
    }

    private static SaleResponseDto MapToDto(Sale s)
    {
        return new SaleResponseDto(
            s.SaleId,
            s.SaleNumber,
            s.Customer?.CustomerName,
            s.CurrencyCode,
            s.Subtotal,
            s.VatTotal,
            s.DiscountTotal,
            s.Total,
            s.PaymentStatus.ToString(),
            s.SaleDate,
            s.SaleItems.Select(si => new SaleItemResponseDto(
                si.SaleItemId, si.ProductId, si.Product?.ProductName ?? "", si.Quantity,
                si.UnitPrice, si.UnitCost, si.VatRate, si.VatAmount, si.LineTotal
            )).ToList(),
            s.Payments.Select(p => new PaymentResponseDto(
                p.PaymentId, p.PaymentMethod.ToString(), p.CurrencyCode, p.Amount,
                p.ReferenceNumber, p.PaymentDate
            )).ToList()
        );
    }
}
