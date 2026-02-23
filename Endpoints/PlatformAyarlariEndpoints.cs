using Api.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class PlatformAyarlariEndpoints
{
    public static RouteGroupBuilder MapPlatformAyarlariEndpoints(this RouteGroupBuilder v1)
    {
        // Ayarlar: tümü veya platforma göre filtre
        v1.MapGet("/platform-ayarlari", async (int? platformId, AppDbContext db) =>
        {
            var q = db.PlatformAyarlari.AsNoTracking().AsQueryable();
            if (platformId is not null) q = q.Where(x => x.PlatformId == platformId);

            var list = await q
                .OrderBy(x => x.PlatformId).ThenBy(x => x.Key)
                .Select(x => new PlatformAyariDto(x.Id, x.PlatformId, x.Key, x.Value, x.Aciklama))
                .ToListAsync();

            return Results.Ok(list);
        });

        // Tek ayar
        v1.MapGet("/platform-ayarlari/{platformId:int}/{key}", async (int platformId, string key, AppDbContext db) =>
        {
            var k = key.Trim();

            var entity = await db.PlatformAyarlari
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlatformId == platformId && x.Key == k);

            return entity is null
                ? Results.NotFound()
                : Results.Ok(new PlatformAyariDto(entity.Id, entity.PlatformId, entity.Key, entity.Value, entity.Aciklama));
        });

        // Upsert ayar
        v1.MapPut("/platform-ayarlari/{platformId:int}/{key}", async (int platformId, string key, UpsertPlatformAyariRequest req, AppDbContext db) =>
        {
            var k = key.Trim();

            var platformExists = await db.Platformlar.AnyAsync(x => x.Id == platformId);
            if (!platformExists) return Results.NotFound(new { message = "Platform bulunamadı." });

            var entity = await db.PlatformAyarlari
                .FirstOrDefaultAsync(x => x.PlatformId == platformId && x.Key == k);

            if (entity is null)
            {
                entity = new Domain.PlatformAyari
                {
                    PlatformId = platformId,
                    Key = k,
                    Value = req.Value,
                    Aciklama = req.Aciklama,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.PlatformAyarlari.Add(entity);
            }
            else
            {
                entity.Value = req.Value;
                entity.Aciklama = req.Aciklama;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Delete ayar
        v1.MapDelete("/platform-ayarlari/{platformId:int}/{key}", async (int platformId, string key, AppDbContext db) =>
        {
            var k = key.Trim();

            var entity = await db.PlatformAyarlari
                .FirstOrDefaultAsync(x => x.PlatformId == platformId && x.Key == k);

            if (entity is null) return Results.NotFound();

            db.PlatformAyarlari.Remove(entity);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return v1;
    }
}