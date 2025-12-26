using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ContentModels.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Entries.Api.Tests;

public class EntriesApiTests : IClassFixture<EntriesApiFactory>
{
    private readonly HttpClient _client;

    public EntriesApiTests(EntriesApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task GetEntries_ReturnsUnauthorizedWithoutAuth()
    {
        var response = await _client.GetAsync("/stacks/00000000-0000-0000-0000-000000000000/tenants/00000000-0000-0000-0000-000000000000/branches/00000000-0000-0000-0000-000000000000/content-models/00000000-0000-0000-0000-000000000000/entries");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
