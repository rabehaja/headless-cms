using System;
using System.Collections.Generic;
using ContentModels.Api;
using ContentModels.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ContentModels.Api.Tests;

public class ContentModelsApiFactory : WebApplicationFactory<Program>
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
                ["ApiKeys:Management"] = "management-key",
                ["UseInMemory"] = "true"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ContentModelsDbContext>));
            services.RemoveAll<ContentModelsDbContext>();
            services.AddDbContext<ContentModelsDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options =>
                {
                    options.TimeProvider = TimeProvider.System;
                });
            services.AddAuthorization();
        });
    }
}
