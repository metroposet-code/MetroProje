using System.Net.Http.Headers;
using System.Text.Json;

namespace Integration.Shopify;

public sealed class ShopifyClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ShopifyClient(HttpClient http)
    {
        _http = http;
    }

    public void Configure(string storeUrl, string accessToken)
    {
        // storeUrl: w5km2d-by.myshopify.com  -> https://w5km2d-by.myshopify.com
        storeUrl = storeUrl.Trim();
        if (!storeUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !storeUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            storeUrl = "https://" + storeUrl;
        }

        storeUrl = storeUrl.TrimEnd('/');

        _http.BaseAddress = new Uri(storeUrl);
        _http.DefaultRequestHeaders.Clear();
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);
    }

    public async IAsyncEnumerable<ShopifyProductDto> GetAllProductsAsync(
        string apiVersion,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        // First page
        var nextUrl = $"/admin/api/{apiVersion}/products.json?limit=250";

        while (!string.IsNullOrWhiteSpace(nextUrl))
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, nextUrl);
            using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var payload = await JsonSerializer.DeserializeAsync<ShopifyProductsResponse>(stream, JsonOptions, ct)
                          ?? new ShopifyProductsResponse();

            foreach (var p in payload.Products)
                yield return p;

            nextUrl = TryGetNextLink(resp);
        }
    }

    private static string? TryGetNextLink(HttpResponseMessage resp)
    {
        // Shopify pagination: Link: <https://.../products.json?limit=250&page_info=...>; rel="next", <...>; rel="previous"
        if (!resp.Headers.TryGetValues("Link", out var values))
            return null;

        var linkHeader = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(linkHeader))
            return null;

        // parse segments separated by comma
        foreach (var part in linkHeader.Split(','))
        {
            var section = part.Trim();
            if (!section.Contains("rel=\"next\"", StringComparison.OrdinalIgnoreCase))
                continue;

            var start = section.IndexOf('<');
            var end = section.IndexOf('>');
            if (start < 0 || end <= start) return null;

            var url = section.Substring(start + 1, end - start - 1);

            // Return as relative if possible
            if (Uri.TryCreate(url, UriKind.Absolute, out var abs))
                return abs.PathAndQuery;

            return url;
        }

        return null;
    }
}