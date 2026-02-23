using Api.Endpoints;
using Endpoints;
using Infrastructure;
using Integration.Shopify;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registrations MUST be before builder.Build()
builder.Services.AddScoped<PlatformSettingsService>();
builder.Services.AddScoped<ShopifySyncService>();
builder.Services.AddHttpClient<ShopifyClient>(c =>
{
    c.Timeout = TimeSpan.FromSeconds(100);
});

var app = builder.Build();

// Swagger'ı sadece Development'ta açmak daha doğru
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// endpoint mappings after Build()
app.MapShopifySyncEndpoints();

// (Opsiyonel) HTTPS redirect isterseniz açın
// app.UseHttpsRedirection();

app.MapGet("/health/db", async (AppDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new { canConnect });
});

// v1 endpoint'lerinin tamamını ayrı cs dosyalarından yükle
app.MapApiV1();
app.Run();