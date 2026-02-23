using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PlatformGrubu> PlatformGruplari => Set<PlatformGrubu>();
    public DbSet<Platform> Platformlar => Set<Platform>();
    public DbSet<PlatformAyari> PlatformAyarlari => Set<PlatformAyari>();

    // Shopify dosyasında implement edilecek (isterseniz boş kalabilir)
    partial void ConfigureShopify(ModelBuilder modelBuilder);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlatformGrubu>(b =>
        {
            b.ToTable("PlatformGruplari", "dbo");
            b.HasKey(x => x.Id);
            b.Property(x => x.GrupAdi).HasMaxLength(100).IsRequired();
            b.Property(x => x.Aciklama).HasMaxLength(500);

            b.HasIndex(x => x.GrupAdi).IsUnique();

            b.HasMany(x => x.Platformlar)
             .WithOne(x => x.Grup)
             .HasForeignKey(x => x.GrupId);
        });

        modelBuilder.Entity<Platform>(b =>
        {
            b.ToTable("Platformlar", "dbo");
            b.HasKey(x => x.Id);

            b.Property(x => x.PlatformAdi).HasMaxLength(100).IsRequired();
            b.Property(x => x.Aciklama).HasMaxLength(500);
            b.Property(x => x.IsActive).IsRequired();

            b.HasIndex(x => x.PlatformAdi).IsUnique();

            b.HasMany(x => x.Ayarlar)
             .WithOne(x => x.Platform)
             .HasForeignKey(x => x.PlatformId);
        });

        modelBuilder.Entity<PlatformAyari>(b =>
        {
            b.ToTable("PlatformAyarlari", "dbo");
            b.HasKey(x => x.Id);

            b.Property(x => x.Key).HasColumnName("Key").HasMaxLength(100).IsRequired();
            b.Property(x => x.Value).HasColumnName("Value");
            b.Property(x => x.Aciklama).HasMaxLength(500);

            b.HasIndex(x => new { x.PlatformId, x.Key }).IsUnique();
        });

        // Shopify tablolarının mapping'leri
        ConfigureShopify(modelBuilder);

    }
}