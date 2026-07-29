namespace SmeRetail.Models.ValueObjects;

public record Money(decimal Amount, string CurrencyCode)
{
    public static Money Zero(string currencyCode) => new(0, currencyCode);
    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new InvalidOperationException("Cannot add money in different currencies.");
        return new Money(Amount + other.Amount, CurrencyCode);
    }
    public Money Multiply(decimal factor) => new(Amount * factor, CurrencyCode);
}
