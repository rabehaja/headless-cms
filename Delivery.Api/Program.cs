using System.Text;
using ContentModels.Domain.Repositories;
using Delivery.Api.Application;
using Delivery.Api.Data;
using Delivery.Api.Data.Repositories;
using Delivery.Api.Middleware;
using Delivery.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<DeliveryDbContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
    if (useInMemory)
    {
        options.UseInMemoryDatabase("DeliveryTest");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("DeliveryDb");
        options.UseNpgsql(connectionString);
    }
});
builder.Services.AddScoped<DeliveryService>();
builder.Services.AddScoped<IDeliveryEntryRepository, DeliveryEntryRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseMiddleware<DeliveryApiKeyMiddleware>();
app.UseMiddleware<ETagMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
