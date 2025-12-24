using System.Security.Claims;

namespace Delivery.Api.Security;

public class DeliveryApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string HeaderName = "X-Delivery-Key";

    public DeliveryApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (context.User?.Identity is { IsAuthenticated: true })
        {
            await _next(context);
            return;
        }

        var configuredKey = configuration["ApiKeys:Delivery"];
        if (!string.IsNullOrWhiteSpace(configuredKey) && context.Request.Headers.TryGetValue(HeaderName, out var providedKey))
        {
            if (string.Equals(providedKey, configuredKey, StringComparison.Ordinal))
            {
                var claims = new List<Claim> { new("auth_type", "apikey"), new("role", "delivery") };
                var identity = new ClaimsIdentity(claims, "ApiKey");
                context.User = new ClaimsPrincipal(identity);
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
    }
}
