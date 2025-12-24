using ContentModels.Domain.Repositories;
using Entries.Api.Application;
using Entries.Api.Background;
using Entries.Api.Data;
using Entries.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<EntriesDbContext>(options =>
{
    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
    if (useInMemory)
    {
        options.UseInMemoryDatabase("EntriesTest");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("EntriesDb");
        options.UseNpgsql(connectionString);
    }
});
builder.Services.AddScoped<EntryService>();
builder.Services.AddScoped<IEntryRepository, EntryRepository>();
builder.Services.AddScoped<EntrySchedulingService>();
builder.Services.AddHostedService<EntryScheduler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program;
