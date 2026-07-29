namespace SmeRetail.Dtos;

public record CreateProductDto(
    string Sku,
    string ProductName,
    string? Barcode,
    string? Description,
    int CategoryId,
    int UomId,
    int VatCategoryId,
    bool TrackInventory = true
);

public record UpdateProductDto(
    string ProductName,
    string? Description,
    int CategoryId,
    int VatCategoryId,
    bool IsActive
);

public record ProductPriceDto(
    string CurrencyCode,
    string PriceType,
    decimal Price
);

public record ProductResponseDto(
    int ProductId,
    string Sku,
    string? Barcode,
    string ProductName,
    string? Description,
    string CategoryName,
    string UomCode,
    decimal VatRate,
    bool TrackInventory,
    bool IsActive,
    List<ProductPriceDto> Prices
);
