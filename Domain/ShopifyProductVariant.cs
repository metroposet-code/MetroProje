namespace Domain;

public sealed class ShopifyProductVariant
{
    public int Id { get; set; }

    public int PlatformId { get; set; }
    public int ProductId { get; set; }
    public ShopifyProduct Product { get; set; } = default!;

    public long ShopifyVariantId { get; set; }
    public long ShopifyProductId { get; set; } // debug/kolay sorgu için (opsiyonel)

    public string? Title { get; set; }
    public string? Sku { get; set; }
    public string? Barcode { get; set; }

    public decimal? Price { get; set; }
    public decimal? CompareAtPrice { get; set; }

    public int? InventoryQuantity { get; set; }
    public long? InventoryItemId { get; set; }

    public string? InventoryManagement { get; set; }
    public string? InventoryPolicy { get; set; }

    public bool Taxable { get; set; }
    public bool RequiresShipping { get; set; }

    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }

    public int? Position { get; set; }

    public string? Option1 { get; set; }
    public string? Option2 { get; set; }
    public string? Option3 { get; set; }

    public long? ImageId { get; set; } // Shopify image id referansı

    public DateTimeOffset? CreatedAtShopify { get; set; }
    public DateTimeOffset? UpdatedAtShopify { get; set; }

    // Soft-delete
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    // Audit
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}