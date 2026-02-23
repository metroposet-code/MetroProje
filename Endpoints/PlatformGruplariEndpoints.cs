using Api.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Api.Endpoints;

public static class PlatformGruplariEndpoints
{
    public static RouteGroupBuilder MapPlatformGruplariEndpoints(this RouteGroupBuilder v1)
    {
        v1.MapGet("/platform-gruplari", async (AppDbContext db) =>
        {
            var list = await db.PlatformGruplari
                .AsNoTracking()
                .OrderBy(x => x.GrupAdi)
                .Select(x => new PlatformGrubuDto(x.Id, x.GrupAdi, x.Aciklama))
                .ToListAsync();

            return Results.Ok(list);
        });

        v1.MapPut("/platform-gruplari/{id:int}", async (int id, UpdatePlatformGrubuRequest req, AppDbContext db) =>
        {
            var entity = await db.PlatformGruplari.FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return Results.NotFound();

            entity.GrupAdi = req.GrupAdi;
            entity.Aciklama = req.Aciklama;
            entity.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return v1;
    }
}