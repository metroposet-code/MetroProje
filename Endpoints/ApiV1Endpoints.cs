using Microsoft.AspNetCore.Builder;

namespace Api.Endpoints;

public static class ApiV1Endpoints
{
    public static IEndpointRouteBuilder MapApiV1(this IEndpointRouteBuilder app)
    {
        var v1 = app.MapGroup("/api/v1");

        v1.MapPlatformGruplariEndpoints();
        v1.MapPlatformlarEndpoints();
        v1.MapPlatformAyarlariEndpoints();

        return app;
    }
}