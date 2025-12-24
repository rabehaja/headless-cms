using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Workflows.Api.Application;
using Workflows.Api.Data;
using Workflows.Api.Data.Repositories;
using Workflows.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<WorkflowsDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("WorkflowsDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<WorkflowService>();
builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<IWebhookNotifier, WebhookNotifier>();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
