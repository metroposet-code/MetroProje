using Api.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class PlatformlarEndpoints
{
    public static RouteGroupBuilder MapPlatformlarEndpoints(this RouteGroupBuilder v1)
    {
        v1.MapGet("/platformlar", async (int? grupId, string? platformKodu, AppDbContext db) =>
        {
            var q = db.Platformlar.AsNoTracking().AsQueryable();

            if (grupId is not null) q = q.Where(x => x.GrupId == grupId);

            if (!string.IsNullOrWhiteSpace(platformKodu))
            {
                var kod = platformKodu.Trim().ToLowerInvariant();
                q = q.Where(x => x.PlatformKodu == kod);
            }

            var list = await q
                .OrderBy(x => x.PlatformAdi)
                .Select(x => new PlatformDto(x.Id, x.GrupId, x.PlatformKodu, x.PlatformAdi, x.Aciklama, x.IsActive))
                .ToListAsync();

            return Results.Ok(list);
        });

        v1.MapPut("/platformlar/{id:int}", async (int id, UpdatePlatformRequest req, AppDbContext db) =>
        {
            var entity = await db.Platformlar.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return Results.NotFound();

            entity.GrupId = req.GrupId;
            entity.PlatformAdi = req.PlatformAdi;
            entity.Aciklama = req.Aciklama;
            entity.IsActive = req.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        v1.MapGet("/platformlar/{platformKodu}/ayarlar", async (string platformKodu, AppDbContext db) =>
        {
            var kod = platformKodu.Trim().ToLowerInvariant();

            var platform = await db.Platformlar
                .AsNoTracking()
                .Select(p => new { p.Id, p.PlatformKodu })
                .FirstOrDefaultAsync(p => p.PlatformKodu == kod);

            if (platform is null) return Results.NotFound(new { message = "Platform bulunamadı." });

            var list = await db.PlatformAyarlari
                .AsNoTracking()
                .Where(x => x.PlatformId == platform.Id)
                .OrderBy(x => x.Key)
                .Select(x => new PlatformAyariDto(x.Id, x.PlatformId, x.Key, x.Value, x.Aciklama))
                .ToListAsync();

            return Results.Ok(list);
        });

        return v1;
    }
}