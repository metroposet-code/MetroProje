using Integration.Shopify;

namespace Endpoints;

public static class ShopifySyncEndpoints
{
    public static IEndpointRouteBuilder MapShopifySyncEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/sync/shopify/products", async (ShopifySyncService svc, CancellationToken ct) =>
        {
            var result = await svc.SyncProductsAsync(ct);
            return Results.Ok(result);
        });

        return app;
    }
}