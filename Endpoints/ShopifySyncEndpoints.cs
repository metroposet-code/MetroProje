// Endpoint for syncing products with Shopify
[HttpGet("/shopify/sync-products")]
public async Task<IActionResult> SyncProductsAsync(bool debug = false, CancellationToken ct = default)
{
    // Call the ShopifySyncService with the debug parameter
    var result = await _shopifySyncService.SyncProductsAsync(debug, ct);
    return Ok(result);
}