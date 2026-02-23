using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed partial class AppDbContext
{
    public DbSet<ShopifyProduct> ShopifyProducts => Set<ShopifyProduct>();
    public DbSet<ShopifyProductVariant> ShopifyProductVariants => Set<ShopifyProductVariant>();
    public DbSet<ShopifyProductImage> ShopifyProductImages => Set<ShopifyProductImage>();

    partial void ConfigureShopify(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShopifyProduct>(b =>
        {
            b.ToTable("ShopifyProducts", "dbo");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PlatformId, x.ShopifyProductId }).IsUnique();

            b.Property(x => x.Title).HasMaxLength(512).IsRequired();
            b.Property(x => x.BodyHtml);

            b.Property(x => x.Vendor).HasMaxLength(256);
            b.Property(x => x.ProductType).HasMaxLength(256);
            b.Property(x => x.Handle).HasMaxLength(512);
            b.Property(x => x.Tags);
            b.Property(x => x.Status).HasMaxLength(64);
            b.Property(x => x.AdminGraphqlApiId).HasMaxLength(128);
            b.Property(x => x.FeaturedImageSrc).HasMaxLength(2048);
        });

        modelBuilder.Entity<ShopifyProductVariant>(b =>
        {
            b.ToTable("ShopifyProductVariants", "dbo");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PlatformId, x.ShopifyVariantId }).IsUnique();

            b.HasOne(x => x.Product)
                .WithMany(x => x.Variants)
                .HasForeignKey(x => x.ProductId);

            b.Property(x => x.Title).HasMaxLength(512);
            b.Property(x => x.Sku).HasMaxLength(128);
            b.Property(x => x.Barcode).HasMaxLength(128);

            b.Property(x => x.InventoryManagement).HasMaxLength(64);
            b.Property(x => x.InventoryPolicy).HasMaxLength(64);
            b.Property(x => x.WeightUnit).HasMaxLength(32);

            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
            b.Property(x => x.CompareAtPrice).HasColumnType("decimal(18,2)");
            b.Property(x => x.Weight).HasColumnType("decimal(18,3)");
        });

        modelBuilder.Entity<ShopifyProductImage>(b =>
        {
            b.ToTable("ShopifyProductImages", "dbo");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.PlatformId, x.ShopifyImageId }).IsUnique();

            b.HasOne(x => x.Product)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId);

            b.Property(x => x.Src).HasMaxLength(2048).IsRequired();
            b.Property(x => x.Alt).HasMaxLength(512);
            b.Property(x => x.VariantIdsJson);
        });
    }
}