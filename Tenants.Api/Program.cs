using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Tenants.Api.Application;
using Tenants.Api.Data;
using Tenants.Api.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TenantsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TenantsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<StackService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IStackRepository, StackRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
