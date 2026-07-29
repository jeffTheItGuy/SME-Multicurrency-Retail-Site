using SmeRetail.Models.Enums;

namespace SmeRetail.Models.Entities;

public class ExchangeRate
{
    public int RateId { get; set; }
    public string FromCurrencyCode { get; set; } = string.Empty;
    public string ToCurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public RateType RateType { get; set; }
    public DateTime EffectiveDate { get; set; }

    public Currency FromCurrency { get; set; } = null!;
    public Currency ToCurrency { get; set; } = null!;
}
