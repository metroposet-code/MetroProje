namespace Domain;

public sealed class ShopifyProduct
{
    public int Id { get; set; }

    public int PlatformId { get; set; } // Platformlar.Id (shopify=2)

    public long ShopifyProductId { get; set; }

    public string Title { get; set; } = default!;
    public string? BodyHtml { get; set; }
    public string? Vendor { get; set; }
    public string? ProductType { get; set; }
    public string? Handle { get; set; }
    public string? Tags { get; set; }
    public string? Status { get; set; }
    public string? AdminGraphqlApiId { get; set; }
    public string? FeaturedImageSrc { get; set; }
    public DateTimeOffset? CreatedAtShopify { get; set; }
    public DateTimeOffset? UpdatedAtShopify { get; set; }
    public DateTimeOffset? PublishedAtShopify { get; set; }

    // Soft-delete
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    // Audit
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<ShopifyProductVariant> Variants { get; set; } = new();
    public List<ShopifyProductImage> Images { get; set; } = new();
}