using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ContentModels.Domain;
using ContentModels.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Webhooks.Api.Services;
using Xunit;

namespace Webhooks.Api.Tests;

public class WebhookDispatcherTests
{
    [Fact]
    public async Task Dispatcher_Queues_Targets_For_Event()
    {
        var repo = new Mock<IWebhookRepository>();
        repo.Setup(r => r.GetByTenantAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WebhookSubscription>
            {
                new() { TenantId = Guid.NewGuid(), Url = "http://localhost", Events = new List<string>{ "entry.published" }, Secret = "secret" }
            });

        var queue = new WebhookQueue();
        var dispatcher = new WebhookDispatcher(queue, repo.Object, new HttpClientFactoryMock(), Mock.Of<ILogger<WebhookDispatcher>>());

        var cts = new CancellationTokenSource();
        var runTask = dispatcher.StartAsync(cts.Token);

        await queue.EnqueueAsync(new WebhookEvent(Guid.NewGuid(), "entry.published", new { id = 1 }), CancellationToken.None);
        await Task.Delay(200);
        cts.Cancel();
        await Task.WhenAny(runTask, Task.Delay(500));

        repo.Verify(r => r.GetByTenantAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    private class HttpClientFactoryMock : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            return new HttpClient(handler.Object);
        }
    }
}
