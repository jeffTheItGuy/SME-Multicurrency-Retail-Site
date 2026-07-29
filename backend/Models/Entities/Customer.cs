namespace SmeRetail.Models.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public decimal CreditLimit { get; set; } = 0;
    public string CurrencyCode { get; set; } = "USD";
    public bool IsActive { get; set; } = true;

    public Currency Currency { get; set; } = null!;
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
