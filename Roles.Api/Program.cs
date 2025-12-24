using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Roles.Api.Application;
using Roles.Api.Data;
using Roles.Api.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RolesDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("RolesDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<ApiKeyService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
