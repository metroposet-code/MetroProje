// Minimal API endpoint extension structure restored
public static class ShopifySyncEndpoints
{
    public static void MapShopifySyncEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/sync-products", async (bool debug, CancellationToken ct) =>
        {
            await ShopifySyncService.SyncProductsAsync(debug, ct);
            return Results.Ok();
        });
    }
}