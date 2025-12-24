using Microsoft.AspNetCore.Mvc;
using Tenants.Api.Data;

namespace Tenants.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly TenantsDbContext _db;

    public HealthController(TenantsDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var dbOk = await _db.Database.CanConnectAsync();
            var result = new
            {
                status = dbOk ? "ok" : "degraded",
                checks = new
                {
                    database = dbOk ? "ok" : "unreachable"
                }
            };

            return dbOk ? Ok(result) : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
        catch (Exception ex)
        {
            var errorResult = new
            {
                status = "error",
                error = ex.Message,
                details = ex.ToString()
            };
            return StatusCode(StatusCodes.Status500InternalServerError, errorResult);
        }
    }
}
