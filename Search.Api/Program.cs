using ContentModels.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Search.Api.Application;
using Search.Api.Data;
using Search.Api.Data.Repositories;
using Search.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<SearchDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SearchDb");
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<TaxonomyService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<ITaxonomyRepository, TaxonomyRepository>();
builder.Services.AddScoped<ISearchIndexRepository, SearchIndexRepository>();
builder.Services.AddSingleton<ElasticSearchClient>();
builder.Services.AddScoped<ElasticIndexer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
