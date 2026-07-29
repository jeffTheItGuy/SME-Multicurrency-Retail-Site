namespace SmeRetail.Models.Entities;

public class Currency
{
    public string CurrencyCode { get; set; } = string.Empty; // PK
    public string CurrencyName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; } = 2;
    public bool IsBaseCurrency { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public ICollection<ExchangeRate> FromRates { get; set; } = new List<ExchangeRate>();
    public ICollection<ExchangeRate> ToRates { get; set; } = new List<ExchangeRate>();
    public ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
}
