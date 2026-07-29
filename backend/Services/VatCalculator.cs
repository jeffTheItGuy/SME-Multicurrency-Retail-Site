using SmeRetail.Models.Entities;
using SmeRetail.Data;
using Microsoft.EntityFrameworkCore;

namespace SmeRetail.Services;

public interface IVatCalculator
{
    Task<(decimal VatAmount, decimal Total)> CalculateVatAsync(int productId, decimal unitPrice, decimal quantity);
    Task<(decimal VatAmount, decimal Total)> CalculateVatAsync(decimal unitPrice, decimal quantity, decimal vatRate);
}

public class VatCalculator : IVatCalculator
{
    private readonly AppDbContext _db;

    public VatCalculator(AppDbContext db) => _db = db;

    public async Task<(decimal VatAmount, decimal Total)> CalculateVatAsync(int productId, decimal unitPrice, decimal quantity)
    {
        var product = await _db.Products
            .Include(p => p.VatCategory)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product?.VatCategory == null) return (0, unitPrice * quantity);

        return CalculateVatAsync(unitPrice, quantity, product.VatCategory.VatRate);
    }

    public Task<(decimal VatAmount, decimal Total)> CalculateVatAsync(decimal unitPrice, decimal quantity, decimal vatRate)
    {
        var lineTotal = unitPrice * quantity;
        var vatAmount = lineTotal * (vatRate / 100m);
        var total = lineTotal + vatAmount;
        return Task.FromResult((vatAmount, total));
    }
}
