using SmeRetail.Models.Enums;

namespace SmeRetail.Dtos;

public record SaleItemDto(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal? DiscountAmount = 0
);

public record CreatePaymentDto(
    PaymentMethod PaymentMethod,
    string CurrencyCode,
    decimal Amount,
    string? ReferenceNumber
);

public record CreateSaleDto(
    int? CustomerId,
    int WarehouseId,
    string CurrencyCode,
    List<SaleItemDto> Items,
    List<CreatePaymentDto>? Payments = null
);

public record SaleItemResponseDto(
    int SaleItemId,
    int ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal? UnitCost,
    decimal VatRate,
    decimal VatAmount,
    decimal LineTotal
);

public record PaymentResponseDto(
    int PaymentId,
    string PaymentMethod,
    string CurrencyCode,
    decimal Amount,
    string? ReferenceNumber,
    DateTime PaymentDate
);

public record SaleResponseDto(
    int SaleId,
    string SaleNumber,
    string? CustomerName,
    string CurrencyCode,
    decimal Subtotal,
    decimal VatTotal,
    decimal DiscountTotal,
    decimal Total,
    string PaymentStatus,
    DateTime SaleDate,
    List<SaleItemResponseDto> Items,
    List<PaymentResponseDto> Payments
);
