using Auth.Api.Application;
using Auth.Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public AuthController(AuthService authService, TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        try
        {
            var user = await _authService.RegisterAsync(request.Email, request.Password, request.Role ?? "viewer", request.TenantId);
            return CreatedAtAction(nameof(Register), new { userId = user.Id }, new { user.Id, user.Email, user.Role, user.TenantId });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authService.ValidateAsync(request.Email, request.Password);
        if (user is null) return Unauthorized(new { message = "Invalid credentials" });

        var token = _tokenService.CreateToken(user.Id, user.TenantId, user.Role, user.Email);
        return Ok(new { token, user = new { user.Id, user.Email, user.Role, user.TenantId } });
    }
}

public record RegisterRequest(string Email, string Password, string? Role, Guid? TenantId);

public record LoginRequest(string Email, string Password);
