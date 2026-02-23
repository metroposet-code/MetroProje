using System.Globalization;
using System.Text.Json;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Integration.Shopify;

public sealed class ShopifySyncService
{
    private const int ShopifyPlatformId = 14;
    private const string ApiVersion = "2026-01";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AppDbContext _db;
    private readonly ShopifyClient _client;
    private readonly PlatformSettingsService _settings;

    public ShopifySyncService(AppDbContext db, ShopifyClient client, PlatformSettingsService settings)
    {
        _db = db;
        _client = client;
        _settings = settings;
    }

    public sealed record SyncResult(
        int ProductsInserted, int ProductsUpdated, int ProductsSoftDeleted,
        int VariantsInserted, int VariantsUpdated, int VariantsSoftDeleted,
        int ImagesInserted, int ImagesUpdated, int ImagesSoftDeleted);

    public async Task<SyncResult> SyncProductsAsync(CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;

        var (storeUrl, token) = await _settings.GetShopifySettingsAsync(ShopifyPlatformId, ct);
        _client.Configure(storeUrl, token);

        // Existing rows (platform scope)
        var existingProducts = await _db.ShopifyProducts
            .Where(x => x.PlatformId == ShopifyPlatformId)
            .ToListAsync(ct);

        var productByShopifyId = existingProducts.ToDictionary(x => x.ShopifyProductId);

        // For variants/images we’ll load later per product to keep memory sane:
        // We'll mark all as "not seen" first, then soft-delete the unseen at end.
        var seenProductIds = new HashSet<long>();

        int pIns = 0, pUpd = 0, pDel = 0;
        int vIns = 0, vUpd = 0, vDel = 0;
        int iIns = 0, iUpd = 0, iDel = 0;

        await foreach (var dto in _client.GetAllProductsAsync(ApiVersion, ct))
        {
            if (dto.Title is null)
                continue;

            seenProductIds.Add(dto.Id);

            if (!productByShopifyId.TryGetValue(dto.Id, out var entity))
            {
                entity = new ShopifyProduct
                {
                    PlatformId = ShopifyPlatformId,
                    ShopifyProductId = dto.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                };
                _db.ShopifyProducts.Add(entity);
                productByShopifyId[dto.Id] = entity;
                pIns++;
            }
            else
            {
                entity.UpdatedAt = now;
                if (entity.IsDeleted)
                {
                    entity.IsDeleted = false;
                    entity.DeletedAt = null;
                }
                pUpd++;
            }

            // Map product fields
            entity.Title = dto.Title!;
            entity.BodyHtml = dto.BodyHtml;
            entity.Vendor = dto.Vendor;
            entity.ProductType = dto.ProductType;
            entity.Handle = dto.Handle;
            entity.Tags = dto.Tags;
            entity.Status = dto.Status;
            entity.AdminGraphqlApiId = dto.AdminGraphqlApiId;
            entity.FeaturedImageSrc = dto.Image?.Src;

            entity.CreatedAtShopify = dto.CreatedAt;
            entity.UpdatedAtShopify = dto.UpdatedAt;
            entity.PublishedAtShopify = dto.PublishedAt;

            // Load existing variants/images for that product (platform scope)
            await _db.Entry(entity)
    .Collection(x => x.Variants)
    .Query()
    .IgnoreQueryFilters()
    .LoadAsync(ct);
            await _db.Entry(entity)
    .Collection(x => x.Images)
    .Query()
    .IgnoreQueryFilters()
    .LoadAsync(ct);

            var variantById = entity.Variants.ToDictionary(x => x.ShopifyVariantId);
            var imageById = entity.Images.ToDictionary(x => x.ShopifyImageId);

            var seenVariantIds = new HashSet<long>();
            foreach (var v in dto.Variants)
            {
                seenVariantIds.Add(v.Id);

                if (!variantById.TryGetValue(v.Id, out var ve))
                {
                    ve = new ShopifyProductVariant
                    {
                        PlatformId = ShopifyPlatformId,
                        Product = entity,
                        ShopifyVariantId = v.Id,
                        ShopifyProductId = dto.Id,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    entity.Variants.Add(ve);
                    variantById[v.Id] = ve;
                    vIns++;
                }
                else
                {
                    ve.UpdatedAt = now;
                    if (ve.IsDeleted)
                    {
                        ve.IsDeleted = false;
                        ve.DeletedAt = null;
                    }
                    vUpd++;
                }

                ve.Title = v.Title;
                ve.Sku = v.Sku;
                ve.Barcode = v.Barcode;

                ve.Price = ParseDecimal(v.Price);
                ve.CompareAtPrice = ParseDecimal(v.CompareAtPrice);

                ve.InventoryQuantity = v.InventoryQuantity;
                ve.InventoryItemId = v.InventoryItemId;
                ve.InventoryManagement = v.InventoryManagement;
                ve.InventoryPolicy = v.InventoryPolicy;

                ve.Taxable = v.Taxable;
                ve.RequiresShipping = v.RequiresShipping;

                ve.Weight = v.Weight;
                ve.WeightUnit = v.WeightUnit;

                ve.Position = v.Position;

                ve.Option1 = v.Option1;
                ve.Option2 = v.Option2;
                ve.Option3 = v.Option3;

                ve.ImageId = v.ImageId;

                ve.CreatedAtShopify = v.CreatedAt;
                ve.UpdatedAtShopify = v.UpdatedAt;
            }

            // Soft-delete variants not seen
            foreach (var ve in entity.Variants)
            {
                if (seenVariantIds.Contains(ve.ShopifyVariantId))
                    continue;

                if (!ve.IsDeleted)
                {
                    ve.IsDeleted = true;
                    ve.DeletedAt = now;
                    ve.UpdatedAt = now;
                    vDel++;
                }
            }

            var seenImageIds = new HashSet<long>();
            foreach (var img in dto.Images)
            {
                if (string.IsNullOrWhiteSpace(img.Src))
                    continue;

                seenImageIds.Add(img.Id);

                if (!imageById.TryGetValue(img.Id, out var ie))
                {
                    // platform genelinde var mı?
                    ie = await _db.ShopifyProductImages
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.PlatformId == ShopifyPlatformId &&
                            x.ShopifyImageId == img.Id, ct);

                    if (ie is null)
                    {
                        ie = new ShopifyProductImage
                        {
                            PlatformId = ShopifyPlatformId,
                            Product = entity,
                            ShopifyImageId = img.Id,
                            ShopifyProductId = dto.Id,
                            Src = img.Src!,
                            CreatedAt = now,
                            UpdatedAt = now
                        };
                        entity.Images.Add(ie);
                        iIns++;
                    }
                    else
                    {
                        // varsa INSERT değil UPDATE: bu ürüne bağla + canlandır
                        ie.Product = entity;
                        ie.ShopifyProductId = dto.Id;
                        ie.UpdatedAt = now;

                        if (ie.IsDeleted)
                        {
                            ie.IsDeleted = false;
                            ie.DeletedAt = null;
                        }

                        iUpd++;
                    }

                    imageById[img.Id] = ie;
                }
                else
                {
                    ie.UpdatedAt = now;
                    if (ie.IsDeleted)
                    {
                        ie.IsDeleted = false;
                        ie.DeletedAt = null;
                    }
                    iUpd++;
                }

                ie.Src = img.Src!;
                ie.Alt = img.Alt;
                ie.Width = img.Width;
                ie.Height = img.Height;
                ie.Position = img.Position;
                ie.VariantIdsJson = img.VariantIds is { Count: > 0 }
                    ? JsonSerializer.Serialize(img.VariantIds, JsonOptions)
                    : "[]";

                ie.CreatedAtShopify = img.CreatedAt;
                ie.UpdatedAtShopify = img.UpdatedAt;
            }

            // Soft-delete images not seen
            foreach (var ie in entity.Images)
            {
                if (seenImageIds.Contains(ie.ShopifyImageId))
                    continue;

                if (!ie.IsDeleted)
                {
                    ie.IsDeleted = true;
                    ie.DeletedAt = now;
                    ie.UpdatedAt = now;
                    iDel++;
                }
            }

            // Flush per N products is possible, but for now:
            await _db.SaveChangesAsync(ct);
        }

        // Soft-delete products not seen
        foreach (var p in existingProducts)
        {
            if (seenProductIds.Contains(p.ShopifyProductId))
                continue;

            if (!p.IsDeleted)
            {
                p.IsDeleted = true;
                p.DeletedAt = now;
                p.UpdatedAt = now;
                pDel++;
            }
        }

        if (pDel > 0)
            await _db.SaveChangesAsync(ct);

        return new SyncResult(pIns, pUpd, pDel, vIns, vUpd, vDel, iIns, iUpd, iDel);
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Shopify returns "1323.00" with dot decimal.
        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d;

        return null;
    }
}