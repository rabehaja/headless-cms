using System;
using System.Collections.Generic;
using Delivery.Api;
using Delivery.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Delivery.Api.Tests;

public class DeliveryApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-key-12345678901234567890",
                ["Jwt:Issuer"] = "cms",
                ["Jwt:Audience"] = "cms-users",
                ["ApiKeys:Delivery"] = "delivery-key",
                ["UseInMemory"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DeliveryDbContext>));
            services.RemoveAll<DeliveryDbContext>();
            services.AddDbContext<DeliveryDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        });
    }
}
