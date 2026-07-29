namespace SmeRetail.Dtos;

public record PoItemDto(
    int ProductId,
    decimal Quantity,
    decimal UnitPrice
);

public record CreatePurchaseOrderDto(
    int SupplierId,
    int WarehouseId,
    string CurrencyCode,
    List<PoItemDto> Items
);

public record GrnItemDto(
    int? PoItemId,
    int ProductId,
    decimal Quantity,
    decimal UnitCost
);

public record CreateGrnDto(
    string GrnNumber,
    int? PoId,
    int SupplierId,
    int WarehouseId,
    string CurrencyCode,
    List<GrnItemDto> Items
);
