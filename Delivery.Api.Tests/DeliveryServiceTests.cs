using ContentModels.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentModels.Domain.Repositories;
using Delivery.Api.Application;
using Moq;
using Xunit;

namespace Delivery.Api.Tests;

public class DeliveryServiceTests
{
    [Fact]
    public async Task Create_Sets_Taxonomies_And_PublishedAt()
    {
        var repo = new Mock<IDeliveryEntryRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<DeliveryEntry>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var svc = new DeliveryService(repo.Object);
        var entry = await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "en-us", true, new Dictionary<string, object?>(), new List<Guid> { Guid.NewGuid() });

        Assert.True(entry.Published);
        Assert.NotNull(entry.PublishedAt);
        Assert.Single(entry.TaxonomyIds);
    }
}
