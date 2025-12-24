using ContentModels.Domain.Repositories;
using Environments.Api.Application;
using Environments.Api.Data;
using Environments.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<EnvironmentsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("EnvironmentsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<EnvironmentService>();
builder.Services.AddScoped<LocaleService>();
builder.Services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<ILocaleRepository, LocaleRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
