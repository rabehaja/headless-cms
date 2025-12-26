using System;
using ContentModels.Domain;
using Xunit;

namespace ContentModels.Domain.Tests;

public class WebhookTests
{
    [Fact]
    public void Webhook_Defaults_Are_Set()
    {
        var hook = new WebhookSubscription
        {
            Name = "Hook",
            Url = "https://example.com",
            TenantId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Secret = "secret"
        };

        Assert.True(hook.Active);
        Assert.Equal(3, hook.MaxRetries);
        Assert.Equal("secret", hook.Secret);
    }
}
