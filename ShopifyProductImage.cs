namespace Domain;

public sealed class ShopifyProductImage
{
    public int Id { get; set; }

    public int PlatformId { get; set; }
    public int ProductId { get; set; }
    public ShopifyProduct Product { get; set; } = default!;

    public long ShopifyImageId { get; set; }
    public long ShopifyProductId { get; set; } // debug/kolay sorgu için (opsiyonel)

    public string Src { get; set; } = default!;
    public string? Alt { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Position { get; set; }

    // Shopify image.variant_ids array
    public string? VariantIdsJson { get; set; }

    public DateTimeOffset? CreatedAtShopify { get; set; }
    public DateTimeOffset? UpdatedAtShopify { get; set; }

    // Soft-delete
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    // Audit
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}