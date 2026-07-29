namespace SmeRetail.Dtos;

public record ExchangeRateDto(
    string FromCurrency,
    string ToCurrency,
    decimal Rate,
    string RateType,
    DateTime EffectiveDate
);

public record FiscaliseInvoiceDto(
    int SaleId
);

public record StockAdjustmentDto(
    int WarehouseId,
    int ProductId,
    decimal QuantityDelta,
    string Reason,
    string CurrencyCode
);

public record InventoryResponseDto(
    int InventoryId,
    int ProductId,
    string ProductName,
    int WarehouseId,
    string WarehouseName,
    decimal Quantity,
    decimal AverageCost,
    string CostCurrency,
    decimal ReorderLevel
);
