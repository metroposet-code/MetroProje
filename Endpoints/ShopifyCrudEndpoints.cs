using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Endpoints;

public static class ShopifyCrudEndpoints
{
    public static IEndpointRouteBuilder MapShopifyCrudEndpoints(this IEndpointRouteBuilder app)
    {
        // ----------------------------
        // ShopifyProducts (DB Id)
        // ----------------------------

        app.MapPut("/api/v1/shopify/products/{id:int}", async (
            int id,
            UpdateShopifyProductRequest request,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProducts.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            // immutable: PlatformId, ShopifyProductId
            entity.Title = request.Title ?? entity.Title;
            entity.BodyHtml = request.BodyHtml;
            entity.Vendor = request.Vendor;
            entity.ProductType = request.ProductType;
            entity.Handle = request.Handle;
            entity.Tags = request.Tags;
            entity.Status = request.Status;
            entity.AdminGraphqlApiId = request.AdminGraphqlApiId;
            entity.FeaturedImageSrc = request.FeaturedImageSrc;

            entity.CreatedAtShopify = request.CreatedAtShopify;
            entity.UpdatedAtShopify = request.UpdatedAtShopify;
            entity.PublishedAtShopify = request.PublishedAtShopify;

            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        app.MapDelete("/api/v1/shopify/products/{id:int}", async (
            int id,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProducts.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            if (!entity.IsDeleted)
            {
                var now = DateTimeOffset.UtcNow;
                entity.IsDeleted = true;
                entity.DeletedAt = now;
                entity.UpdatedAt = now;
                await db.SaveChangesAsync(ct);
            }

            return Results.NoContent();
        });

        // ----------------------------
        // ShopifyProductVariants (DB Id)
        // ----------------------------

        app.MapPut("/api/v1/shopify/variants/{id:int}", async (
            int id,
            UpdateShopifyVariantRequest request,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProductVariants.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            // immutable: PlatformId, ProductId, ShopifyVariantId, ShopifyProductId
            entity.Title = request.Title;
            entity.Sku = request.Sku;
            entity.Barcode = request.Barcode;

            entity.Price = request.Price;
            entity.CompareAtPrice = request.CompareAtPrice;

            entity.InventoryQuantity = request.InventoryQuantity;
            entity.InventoryItemId = request.InventoryItemId;
            entity.InventoryManagement = request.InventoryManagement;
            entity.InventoryPolicy = request.InventoryPolicy;

            entity.Taxable = request.Taxable;
            entity.RequiresShipping = request.RequiresShipping;

            entity.Weight = request.Weight;
            entity.WeightUnit = request.WeightUnit;

            entity.Position = request.Position;
            entity.Option1 = request.Option1;
            entity.Option2 = request.Option2;
            entity.Option3 = request.Option3;

            entity.ImageId = request.ImageId;

            entity.CreatedAtShopify = request.CreatedAtShopify;
            entity.UpdatedAtShopify = request.UpdatedAtShopify;

            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        app.MapDelete("/api/v1/shopify/variants/{id:int}", async (
            int id,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProductVariants.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            if (!entity.IsDeleted)
            {
                var now = DateTimeOffset.UtcNow;
                entity.IsDeleted = true;
                entity.DeletedAt = now;
                entity.UpdatedAt = now;
                await db.SaveChangesAsync(ct);
            }

            return Results.NoContent();
        });

        // ----------------------------
        // ShopifyProductImages (DB Id)
        // ----------------------------

        app.MapPut("/api/v1/shopify/images/{id:int}", async (
            int id,
            UpdateShopifyImageRequest request,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProductImages.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            // immutable: PlatformId, ProductId, ShopifyImageId, ShopifyProductId
            entity.Src = request.Src ?? entity.Src;
            entity.Alt = request.Alt;
            entity.Width = request.Width;
            entity.Height = request.Height;
            entity.Position = request.Position;
            entity.VariantIdsJson = request.VariantIdsJson;

            entity.CreatedAtShopify = request.CreatedAtShopify;
            entity.UpdatedAtShopify = request.UpdatedAtShopify;

            entity.UpdatedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        app.MapDelete("/api/v1/shopify/images/{id:int}", async (
            int id,
            AppDbContext db,
            CancellationToken ct) =>
        {
            var entity = await db.ShopifyProductImages.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return Results.NotFound();

            if (!entity.IsDeleted)
            {
                var now = DateTimeOffset.UtcNow;
                entity.IsDeleted = true;
                entity.DeletedAt = now;
                entity.UpdatedAt = now;
                await db.SaveChangesAsync(ct);
            }

            return Results.NoContent();
        });

        return app;
    }

    // ----------------------------
    // Request DTOs
    // ----------------------------

    public sealed record UpdateShopifyProductRequest(
        string? Title,
        string? BodyHtml,
        string? Vendor,
        string? ProductType,
        string? Handle,
        string? Tags,
        string? Status,
        string? AdminGraphqlApiId,
        string? FeaturedImageSrc,
        DateTimeOffset? CreatedAtShopify,
        DateTimeOffset? UpdatedAtShopify,
        DateTimeOffset? PublishedAtShopify
    );

    public sealed record UpdateShopifyVariantRequest(
        string? Title,
        string? Sku,
        string? Barcode,
        decimal? Price,
        decimal? CompareAtPrice,
        int? InventoryQuantity,
        long? InventoryItemId,
        string? InventoryManagement,
        string? InventoryPolicy,
        bool Taxable,
        bool RequiresShipping,
        decimal? Weight,
        string? WeightUnit,
        int? Position,
        string? Option1,
        string? Option2,
        string? Option3,
        long? ImageId,
        DateTimeOffset? CreatedAtShopify,
        DateTimeOffset? UpdatedAtShopify
    );

    public sealed record UpdateShopifyImageRequest(
        string? Src,
        string? Alt,
        int? Width,
        int? Height,
        int? Position,
        string? VariantIdsJson,
        DateTimeOffset? CreatedAtShopify,
        DateTimeOffset? UpdatedAtShopify
    );
}