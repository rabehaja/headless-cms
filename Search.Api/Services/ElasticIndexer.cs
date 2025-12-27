using ContentModels.Domain;
using Elastic.Clients.Elasticsearch;

namespace Search.Api.Services;

public class ElasticIndexer
{
    private readonly ElasticSearchClient _client;
    private readonly ILogger<ElasticIndexer> _logger;

    public ElasticIndexer(ElasticSearchClient client, ILogger<ElasticIndexer> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task IndexAsync(SearchIndexItem item, CancellationToken cancellationToken = default)
    {
        var response = await _client.Client.IndexAsync(item, idx => idx.Index(_client.IndexName).Id(item.Id), cancellationToken);
        if (!response.IsValidResponse)
        {
            _logger.LogWarning("Failed to index entry {EntryId}: {Error}", item.EntryId, response.ElasticsearchServerError?.ToString() ?? "Unknown");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _client.Client.DeleteAsync<SearchIndexItem>(id, d => d.Index(_client.IndexName), cancellationToken);
    }

    public async Task<List<SearchIndexItem>> SearchAsync(Guid tenantId, string[] terms, Guid? contentModelId, string? locale, int skip, int take, CancellationToken cancellationToken = default)
    {
        var response = await _client.Client.SearchAsync<SearchIndexItem>(s => s
            .Index(_client.IndexName)
            .Query(q => q.Bool(b =>
            {
                b.Must(m => m.Term(t => t.Field(new Field("tenantId")).Value(tenantId.ToString())));

                if (!string.IsNullOrWhiteSpace(locale))
                {
                    b.Must(m => m.Term(t => t.Field(new Field("locale")).Value(locale)));
                }

                if (contentModelId is not null)
                {
                    b.Must(m => m.Term(t => t.Field(new Field("contentModelId")).Value(contentModelId.Value.ToString())));
                }

                if (terms.Length > 0)
                {
                    foreach (var term in terms)
                    {
                        var captured = term;
                        b.Must(m => m.Match(mm => mm.Field(f => f.Text).Query(captured)));
                    }
                }
            }))
            .From(skip)
            .Size(take), cancellationToken);

        return response.Hits.Select(h => h.Source!).ToList();
    }
}
