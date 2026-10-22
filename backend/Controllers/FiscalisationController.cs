using Microsoft.AspNetCore.Mvc;
using SmeRetail.Dtos;
using SmeRetail.Services;

namespace SmeRetail.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FiscalisationController : ControllerBase
{
    private readonly IFiscalisationService _fiscal;

    public FiscalisationController(IFiscalisationService fiscal)
    {
        _fiscal = fiscal;
    }

    [HttpPost("fiscalise")]
    public async Task<IActionResult> Fiscalise([FromBody] FiscaliseInvoiceDto dto)
    {
        var invoice = await _fiscal.FiscaliseAsync(dto.SaleId);
        return Ok(invoice);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var pending = await _fiscal.GetPendingSubmissionsAsync();
        return Ok(pending);
    }

    [HttpPost("submit/{invoiceId}")]
    public async Task<IActionResult> MarkSubmitted(int invoiceId, [FromQuery] string fiscalInvoiceNumber, [FromQuery] string ackCode)
    {
        await _fiscal.MarkAsSubmittedAsync(invoiceId, fiscalInvoiceNumber, ackCode);
        return Ok(new { message = "Marked as submitted" });
    }
}
