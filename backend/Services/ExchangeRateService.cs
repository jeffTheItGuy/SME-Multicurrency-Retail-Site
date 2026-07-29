using SmeRetail.Models.Enums;
using SmeRetail.Models.Entities;
using SmeRetail.Data;
using Microsoft.EntityFrameworkCore;

namespace SmeRetail.Services;

public interface IExchangeRateService
{
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency, RateType rateType = RateType.Official);
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, RateType rateType = RateType.Official);
    Task SeedRatesAsync();
}

public class ExchangeRateService : IExchangeRateService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ExchangeRateService> _logger;

    public ExchangeRateService(AppDbContext db, ILogger<ExchangeRateService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<decimal> GetRateAsync(string fromCurrency, string toCurrency, RateType rateType = RateType.Official)
    {
        if (fromCurrency == toCurrency) return 1m;

        var rate = await _db.ExchangeRates
            .Where(r => r.FromCurrencyCode == fromCurrency 
                     && r.ToCurrencyCode == toCurrency 
                     && r.RateType == rateType
                     && r.EffectiveDate <= DateTime.UtcNow)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (rate != null) return rate.Rate;

        // Try inverse
        var inverse = await _db.ExchangeRates
            .Where(r => r.FromCurrencyCode == toCurrency 
                     && r.ToCurrencyCode == fromCurrency 
                     && r.RateType == rateType
                     && r.EffectiveDate <= DateTime.UtcNow)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync();

        if (inverse != null) return 1m / inverse.Rate;

        _logger.LogWarning("Rate not found for {From} -> {To} ({Type})", fromCurrency, toCurrency, rateType);
        return 1m; // Fallback
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, RateType rateType = RateType.Official)
    {
        var rate = await GetRateAsync(fromCurrency, toCurrency, rateType);
        return amount * rate;
    }

    public async Task SeedRatesAsync()
    {
        if (!await _db.ExchangeRates.AnyAsync())
        {
            _db.ExchangeRates.AddRange(
                new ExchangeRate { FromCurrencyCode = "USD", ToCurrencyCode = "ZiG", Rate = 13.5m, RateType = RateType.Official, EffectiveDate = DateTime.UtcNow.Date },
                new ExchangeRate { FromCurrencyCode = "USD", ToCurrencyCode = "ZiG", Rate = 16.2m, RateType = RateType.Parallel, EffectiveDate = DateTime.UtcNow.Date }
            );
            await _db.SaveChangesAsync();
        }
    }
}
