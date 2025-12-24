using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Search.Api.Services;

public class ElasticSearchClient
{
    public ElasticsearchClient Client { get; }
    public string IndexName { get; }

    public ElasticSearchClient(IConfiguration configuration)
    {
        var uri = configuration["Elastic:Uri"] ?? "http://localhost:9200";
        IndexName = configuration["Elastic:Index"] ?? "entries";
        var settings = new ElasticsearchClientSettings(new Uri(uri))
            .DefaultIndex(IndexName);
        Client = new ElasticsearchClient(settings);
    }
}
