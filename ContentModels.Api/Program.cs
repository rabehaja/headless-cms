using System.Text;
using ContentModels.Api.Application;
using ContentModels.Api.Data;
using ContentModels.Api.Data.Repositories;
using ContentModels.Api.Security;
using ContentModels.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ContentModelsDbContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
    if (useInMemory)
    {
        options.UseInMemoryDatabase("ContentModelsTest");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("ContentModelsDb");
        options.UseNpgsql(connectionString);
    }
});
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<StackService>();
builder.Services.AddScoped<ContentModelService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IStackRepository, StackRepository>();
builder.Services.AddScoped<IContentModelRepository, ContentModelRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddHttpContextAccessor();
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
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
