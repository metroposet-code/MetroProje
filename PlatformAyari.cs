namespace Domain;

public sealed class PlatformAyari
{
    public int Id { get; set; }

    public int PlatformId { get; set; }
    public Platform Platform { get; set; } = default!;

    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Aciklama { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}