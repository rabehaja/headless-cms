using Assets.Api.Application;
using Assets.Api.Data;
using Assets.Api.Data.Repositories;
using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AssetsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AssetsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
