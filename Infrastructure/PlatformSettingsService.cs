using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class PlatformSettingsService
{
    private readonly AppDbContext _db;

    public PlatformSettingsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(string storeUrl, string accessToken)> GetShopifySettingsAsync(int platformId, CancellationToken ct)
    {
        var settings = await _db.PlatformAyarlari
            .AsNoTracking()
            .Where(x => x.PlatformId == platformId)
            .ToDictionaryAsync(x => x.Key, x => x.Value, ct);

        if (!settings.TryGetValue("StoreUrl", out var storeUrl) || string.IsNullOrWhiteSpace(storeUrl))
            throw new InvalidOperationException("PlatformAyarlari: StoreUrl bulunamadı.");

        if (!settings.TryGetValue("AccessToken", out var token) || string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("PlatformAyarlari: AccessToken bulunamadı.");

        return (storeUrl.Trim(), token.Trim());
    }
}