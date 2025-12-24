using GlobalFields.Api.Application;
using GlobalFields.Api.Data;
using GlobalFields.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using ContentModels.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<GlobalFieldsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("GlobalFieldsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<GlobalFieldService>();
builder.Services.AddScoped<IGlobalFieldRepository, GlobalFieldRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
