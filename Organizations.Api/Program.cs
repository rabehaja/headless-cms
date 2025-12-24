using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Organizations.Api.Application;
using Organizations.Api.Data;
using Organizations.Api.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<OrganizationsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("OrganizationsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
