using Entries.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Entries.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly EntriesDbContext _db;

    public HealthController(EntriesDbContext db)
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
