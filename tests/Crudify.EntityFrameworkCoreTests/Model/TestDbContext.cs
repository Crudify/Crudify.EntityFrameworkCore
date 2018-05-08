using Microsoft.EntityFrameworkCore;
using System;

namespace Crudify.EntityFrameworkCoreTests.Model
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        { }

        public DbSet<Foo> Foos { get; set; }

        public DbSet<Bar> Bars { get; set; }

        public static TestDbContext CreateInMemoryNoTrackingTestDbContext()
        {
            return CreateInMemoryNoTrackingTestDbContext(Guid.NewGuid().ToString());
        }

        public static TestDbContext CreateInMemoryNoTrackingTestDbContext(string databaseName)
        {
            var testDbContextInMemoryOptions = new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase(databaseName)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .Options;

            return new TestDbContext(testDbContextInMemoryOptions);
        }
    }
}