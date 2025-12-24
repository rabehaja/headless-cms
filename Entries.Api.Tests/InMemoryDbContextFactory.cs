using System;
using Entries.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Entries.Api.Tests;

internal static class InMemoryDbContextFactory
{
    public static EntriesDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EntriesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EntriesDbContext(options);
    }
}
