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
public class ExchangeRatesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IExchangeRateService _svc;

    public ExchangeRatesController(AppDbContext db, IExchangeRateService svc)
    {
        _db = db;
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> GetRates([FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? type)
    {
        var query = _db.ExchangeRates.AsNoTracking();
        if (!string.IsNullOrEmpty(from)) query = query.Where(r => r.FromCurrencyCode == from);
        if (!string.IsNullOrEmpty(to)) query = query.Where(r => r.ToCurrencyCode == to);
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<RateType>(type, true, out var rt)) 
            query = query.Where(r => r.RateType == rt);

        var items = await query.OrderByDescending(r => r.EffectiveDate).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExchangeRateDto dto)
    {
        if (!Enum.TryParse<RateType>(dto.RateType, true, out var rateType))
            return BadRequest("Invalid rate type");

        _db.ExchangeRates.Add(new ExchangeRate
        {
            FromCurrencyCode = dto.FromCurrency,
            ToCurrencyCode = dto.ToCurrency,
            Rate = dto.Rate,
            RateType = rateType,
            EffectiveDate = dto.EffectiveDate
        });

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert([FromQuery] decimal amount, [FromQuery] string from, [FromQuery] string to, [FromQuery] string type = "Official")
    {
        if (!Enum.TryParse<RateType>(type, true, out var rateType))
            return BadRequest("Invalid rate type");

        var result = await _svc.ConvertAsync(amount, from, to, rateType);
        return Ok(new { amount, from, to, type, result });
    }
}
