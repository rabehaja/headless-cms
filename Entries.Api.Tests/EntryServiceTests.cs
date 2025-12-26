using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentModels.Domain;
using System.Threading;
using ContentModels.Domain.Repositories;
using Entries.Api.Application;
using Moq;
using Xunit;

namespace Entries.Api.Tests;

public class EntryServiceTests
{
    [Fact]
    public async Task Create_Sets_State_And_Taxonomies()
    {
        var repo = new Mock<IEntryRepository>();
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.AddAsync(It.IsAny<Entry>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var svc = new EntryService(repo.Object);
        var entry = await svc.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "en-us", new Dictionary<string, object?>(), true, null, new List<Guid> { Guid.NewGuid() });

        Assert.True(entry.Published);
        Assert.Equal(EntryState.Published, entry.State);
        Assert.Single(entry.TaxonomyIds);
    }

    [Fact]
    public async Task Update_Sets_Unpublish_State()
    {
        var entry = new Entry { Published = true, State = EntryState.Published };
        var repo = new Mock<IEntryRepository>();
        repo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entry);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var svc = new EntryService(repo.Object);
        var updated = await svc.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new Dictionary<string, object?>(), false, null, null, null);
        Assert.True(updated);
        Assert.False(entry.Published);
        Assert.Equal(EntryState.Unpublished, entry.State);
    }
}
