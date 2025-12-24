using System.Security.Cryptography;
using System.Text;

namespace Delivery.Api.Middleware;

public class ETagMiddleware
{
    private readonly RequestDelegate _next;

    public ETagMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var originalBody = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status200OK)
        {
            memStream.Position = 0;
            var bodyBytes = memStream.ToArray();
            var etag = ComputeHash(bodyBytes);
            context.Response.Headers.ETag = $"\"{etag}\"";

            if (context.Request.Headers.TryGetValue("If-None-Match", out var inm) && inm.ToString().Trim('"') == etag)
            {
                context.Response.Body = originalBody;
                context.Response.StatusCode = StatusCodes.Status304NotModified;
                return;
            }

            await memStream.CopyToAsync(originalBody);
        }
        else
        {
            memStream.Position = 0;
            await memStream.CopyToAsync(originalBody);
        }
    }

    private static string ComputeHash(byte[] data)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(data);
        return Convert.ToHexString(hash);
    }
}
