using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Delivery.Api.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Delivery.Api.Tests;

public class DeliveryApiTests : IClassFixture<DeliveryApiFactory>
{
    private readonly HttpClient _client;

    public DeliveryApiTests(DeliveryApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task DeliveryEndpoint_Requires_Key()
    {
        var response = await _client.GetAsync("/tenants/00000000-0000-0000-0000-000000000000/branches/00000000-0000-0000-0000-000000000000/content-models/00000000-0000-0000-0000-000000000000/entries");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeliveryEndpoint_With_Key_Allows()
    {
        _client.DefaultRequestHeaders.Add("X-Delivery-Key", "delivery-key");
        var response = await _client.GetAsync("/tenants/00000000-0000-0000-0000-000000000000/branches/00000000-0000-0000-0000-000000000000/content-models/00000000-0000-0000-0000-000000000000/entries");
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
