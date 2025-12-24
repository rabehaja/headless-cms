using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Webhooks.Api.Application;
using Webhooks.Api.Data;
using Webhooks.Api.Data.Repositories;
using Webhooks.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<WebhooksDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("WebhooksDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<WebhookService>();
builder.Services.AddScoped<IWebhookRepository, WebhookRepository>();
builder.Services.AddSingleton<WebhookQueue>();
builder.Services.AddHttpClient("webhooks");
builder.Services.AddHostedService<WebhookDispatcher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
