namespace SmeRetail.Models.Entities;

public class VatRecord
{
    public int VatRecordId { get; set; }
    public int InvoiceId { get; set; }
    public int VatCategoryId { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal VatRate { get; set; }
    public decimal VatAmount { get; set; }
    public string TaxPeriod { get; set; } = string.Empty; // YYYY-MM
    public bool SubmittedToAuthority { get; set; } = false;
    public string? AuthorityAckCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Invoice Invoice { get; set; } = null!;
    public VatCategory VatCategory { get; set; } = null!;
}
