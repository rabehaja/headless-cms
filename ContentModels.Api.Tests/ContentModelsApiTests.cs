using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContentModels.Api.Tests;

public class ContentModelsApiTests : IClassFixture<ContentModelsApiFactory>
{
    private readonly HttpClient _client;

    public ContentModelsApiTests(ContentModelsApiFactory factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // override configuration for tests
            });
        }).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        _client.DefaultRequestHeaders.Add("X-Management-Key", "management-key");
    }

    [Fact]
    public async Task StandardFields_ReturnsOk()
    {
        var response = await _client.GetAsync("/standard-fields");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fields = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(fields);
        Assert.NotEmpty(fields);
    }

    [Fact]
    public async Task ManagementEndpoints_Succeed_WithApiKey()
    {
        _client.DefaultRequestHeaders.Add("X-Management-Key", "management-key");
        var create = await _client.PostAsJsonAsync("/organizations", new { Name = "Org1" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var response = await _client.GetAsync("/organizations");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Can_Create_Tenant_With_ApiKey()
    {
        _client.DefaultRequestHeaders.Add("X-Management-Key", "management-key");
        var org = await _client.PostAsJsonAsync("/organizations", new { Name = "Org2" });
        var orgBody = await org.Content.ReadFromJsonAsync<Organization>();
        Assert.NotNull(orgBody);

        var stackResponse = await _client.PostAsJsonAsync($"/stacks", new { Name = "Stack1", OrganizationId = orgBody!.Id });
        var stack = await stackResponse.Content.ReadFromJsonAsync<ContentModels.Domain.Stack>();
        Assert.NotNull(stack);

        var tenantResponse = await _client.PostAsJsonAsync($"/stacks/{stack!.Id}/tenants", new { Name = "Tenant1" });
        Assert.Equal(HttpStatusCode.Created, tenantResponse.StatusCode);
    }
}
