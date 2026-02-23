using System.Runtime.InteropServices;

namespace Domain;

public sealed class PlatformGrubu
{
    public int Id { get; set; }
    public string GrupAdi { get; set; } = default!;
    public string? Aciklama { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Platform> Platformlar { get; set; } = new();
}