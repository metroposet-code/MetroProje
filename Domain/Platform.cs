namespace Domain;

public sealed class Platform
{
    public int Id { get; set; }
    public int GrupId { get; set; }
    public PlatformGrubu Grup { get; set; } = default!;

    public string PlatformKodu { get; set; } = default!;
    public string PlatformAdi { get; set; } = default!;
    public string? Aciklama { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<PlatformAyari> Ayarlar { get; set; } = new();
}