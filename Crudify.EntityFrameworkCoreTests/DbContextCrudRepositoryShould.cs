using Crudify.EntityFrameworkCore;
using Crudify.EntityFrameworkCoreTests.Model;
using Microsoft.Extensions.Logging;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Crudify.EntityFrameworkCoreTests
{
    public class DbContextCrudRepositoryShould
    {
        private Moq.Mock<ILogger> loggerMock = new Moq.Mock<ILogger>();

        [Fact]
        public void ThrowArgumentNullExceptionInConstructorAIfDbContextIsNull()
        {
            Should.Throw(() => new DbContextCrudRepository<Foo, int>(dbContext: null, logger: loggerMock.Object), typeof(ArgumentNullException));
        }

        [Fact]
        public void ThrowArgumentNullExceptionInConstructorAIfDbLoggerIsNull()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                Should.Throw(() => new DbContextCrudRepository<Foo, int>(dbContext: dbContext, logger: null), typeof(ArgumentNullException));
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionInConstructorBIfDbContextIsNull()
        {
            Should.Throw(() => new DbContextCrudRepository<Foo, int>(dbContext: null, logger: loggerMock.Object, keepDbContextOpenOnDispose: false), typeof(ArgumentNullException));
        }

        [Fact]
        public void ThrowArgumentNullExceptionInConstructorBIfDbLoggerIsNull()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                Should.Throw(() => new DbContextCrudRepository<Foo, int>(dbContext: dbContext, logger: null, keepDbContextOpenOnDispose: false), typeof(ArgumentNullException));
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionInConstructorCIfDbContextCrudRepositoryOptionsIsNull()
        {
            Should.Throw(() => new DbContextCrudRepository<Foo, int>(dbContextCrudRepositoryOptions: null), typeof(ArgumentNullException));
        }

        [Fact]
        public void SetPropertiesCorrectlyWhenUsingConstructorA()
        {
            var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext();
            var loggerObject = loggerMock.Object;
            using (var dbContextCrudRepository = new DbContextCrudRepository<Foo, int>(dbContext: dbContext, logger: loggerObject))
            {
                dbContextCrudRepository.DbContext.ShouldBeSameAs(dbContext);
                dbContextCrudRepository.Logger.ShouldBeSameAs(loggerObject);
                dbContextCrudRepository.KeepDbContextOpenOnDispose.ShouldBeTrue();
            }
        }

        [Fact]
        public void SetPropertiesCorrectlyWhenUsingConstructorB()
        {
            var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext();
            var loggerObject = loggerMock.Object;
            var keepDbContextOpenOnDispose = false;
            using (var dbContextCrudRepository = new DbContextCrudRepository<Foo, int>(dbContext: dbContext, logger: loggerObject, keepDbContextOpenOnDispose: keepDbContextOpenOnDispose))
            {
                dbContextCrudRepository.DbContext.ShouldBeSameAs(dbContext);
                dbContextCrudRepository.Logger.ShouldBeSameAs(loggerObject);
                dbContextCrudRepository.KeepDbContextOpenOnDispose.ShouldBe(keepDbContextOpenOnDispose);
            }
        }

        [Fact]
        public void SetPropertiesCorrectlyWhenUsingConstructorC()
        {
            var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext();
            var loggerObject = loggerMock.Object;
            var keepDbContextOpenOnDispose = false;

            var dbContextCrudRepositoryOptions = new DbContextCrudRepositoryOptions()
            {
                DbContext = dbContext,
                Logger = loggerObject,
                KeepDbContextOpenOnDispose = keepDbContextOpenOnDispose,
            };

            using (var dbContextCrudRepository = new DbContextCrudRepository<Foo, int>(dbContextCrudRepositoryOptions: dbContextCrudRepositoryOptions))
            {
                dbContextCrudRepository.DbContext.ShouldBeSameAs(dbContext);
                dbContextCrudRepository.Logger.ShouldBeSameAs(loggerObject);
                dbContextCrudRepository.KeepDbContextOpenOnDispose.ShouldBe(keepDbContextOpenOnDispose);
            }
        }

        [Fact]
        public void CreateAndReturnAnId()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    dbContextCrudRepository
                        .Create(new Bar() { Stool = Guid.NewGuid().ToString() })
                        .ShouldBeGreaterThan<ulong>(0);
                }
            }
        }

        [Fact]
        public async Task CreateAsyncAndReturnAnId()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    (await dbContextCrudRepository
                        .CreateAsync(new Bar() { Stool = Guid.NewGuid().ToString() }))
                        .ShouldBeGreaterThan<ulong>(0);
                }
            }
        }

        [Fact]
        public void CreateAndAllowSubsequentRead()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var user = new Bar() { Stool = Guid.NewGuid().ToString() };

                    var newUserId = dbContextCrudRepository.Create(user);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = dbContextCrudRepository.Read(newUserId);

                    readResult.ShouldNotBeNull();
                    readResult.Stool.ShouldNotBeNull();
                    readResult.Stool.ShouldBe(user.Stool);
                }
            }
        }

        [Fact]
        public async Task CreateAsyncAndAllowSubsequentRead()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var user = new Bar() { Stool = Guid.NewGuid().ToString() };

                    var newUserId = await dbContextCrudRepository.CreateAsync(user);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = dbContextCrudRepository.Read(newUserId);

                    readResult.ShouldNotBeNull();
                    readResult.Stool.ShouldNotBeNull();
                    readResult.Stool.ShouldBe(user.Stool);
                }
            }
        }

        [Fact]
        public async Task CreateAndAllowSubsequentReadAsync()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var user = new Bar() { Stool = Guid.NewGuid().ToString() };

                    var newUserId = dbContextCrudRepository.Create(user);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = await dbContextCrudRepository.ReadAsync(newUserId);

                    readResult.ShouldNotBeNull();
                    readResult.Stool.ShouldNotBeNull();
                    readResult.Stool.ShouldBe(user.Stool);
                }
            }
        }

        [Fact]
        public async Task CreateAsyncAndAllowSubsequentReadAsync()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = Guid.NewGuid().ToString() };

                    var newUserId = await dbContextCrudRepository.CreateAsync(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = await dbContextCrudRepository.ReadAsync(newUserId);

                    readResult.ShouldNotBeNull();
                    readResult.Stool.ShouldNotBeNull();
                    readResult.Stool.ShouldBe(newUser.Stool);
                }
            }
        }

        [Fact]
        public void CreateThenReadThenUpdateThenReadAsExpected()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = dbContextCrudRepository.Create(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = dbContextCrudRepository.Read(newUserId);
                    readResult.Stool = "After";

                    dbContextCrudRepository.Update(readResult);

                    var readAfterUpdateResult = dbContextCrudRepository.Read(newUserId);

                    readAfterUpdateResult.Stool.ShouldNotBeNull();
                    readAfterUpdateResult.Stool.ShouldBe("After");
                }
            }
        }

        [Fact]
        public async Task CreateThenReadThenUpdateAsyncThenReadAsExpected()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = dbContextCrudRepository.Create(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = dbContextCrudRepository.Read(newUserId);
                    readResult.Stool = "After";

                    await dbContextCrudRepository.UpdateAsync(readResult);

                    var readAfterUpdateResult = dbContextCrudRepository.Read(newUserId);

                    readAfterUpdateResult.Stool.ShouldNotBeNull();
                    readAfterUpdateResult.Stool.ShouldBe("After");
                }
            }
        }

        [Fact]
        public async Task CreateAsyncThenReadThenUpdateThenReadAsExpected()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = await dbContextCrudRepository.CreateAsync(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = dbContextCrudRepository.Read(newUserId);
                    readResult.Stool = "After";

                    dbContextCrudRepository.Update(readResult);

                    var readAfterUpdateResult = dbContextCrudRepository.Read(newUserId);

                    readAfterUpdateResult.Stool.ShouldNotBeNull();
                    readAfterUpdateResult.Stool.ShouldBe("After");
                }
            }
        }

        [Fact]
        public async Task CreateAsyncThenReadAsyncThenUpdateThenReadAsExpected()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = await dbContextCrudRepository.CreateAsync(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    var readResult = await dbContextCrudRepository.ReadAsync(newUserId);
                    readResult.Stool = "After";

                    dbContextCrudRepository.Update(readResult);

                    var readAfterUpdateResult = dbContextCrudRepository.Read(newUserId);

                    readAfterUpdateResult.Stool.ShouldNotBeNull();
                    readAfterUpdateResult.Stool.ShouldBe("After");
                }
            }
        }

        [Fact]
        public async Task CreateAsyncThenDeleteAsExpectedThroughSubsequentNullRead()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = await dbContextCrudRepository.CreateAsync(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    dbContextCrudRepository.Delete(newUserId);

                    dbContextCrudRepository.Read(newUserId).ShouldBeNull();
                }
            }
        }

        [Fact]
        public async Task CreateAsyncThenDeleteAsyncAsExpectedThroughSubsequentNullRead()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                using (var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext))
                {
                    var newUser = new Bar() { Stool = "Before" };
                    var newUserId = await dbContextCrudRepository.CreateAsync(newUser);
                    newUserId.ShouldNotBe<ulong>(0);

                    await dbContextCrudRepository.DeleteAsync(newUserId);

                    dbContextCrudRepository.Read(newUserId).ShouldBeNull();
                }
            }
        }

        [Fact]
        public void NotDisposeOfDbContextWhenDisposed()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext);

                var newUser = new Bar() { Stool = "Before" };
                dbContextCrudRepository.Create(newUser);

                dbContextCrudRepository.Dispose();

                dbContext.Bars.First().ShouldNotBeNull();
            }
        }

        [Fact]
        public void DisposeOfDbContextWhenKeepDbContextOpenOnDisposeIsSetToFalse()
        {
            using (var dbContext = TestDbContext.CreateInMemoryNoTrackingTestDbContext())
            {
                var dbContextCrudRepository = new DbContextCrudRepository<Bar, ulong>(
                    logger: loggerMock.Object,
                    dbContext: dbContext,
                    keepDbContextOpenOnDispose: false);

                dbContextCrudRepository.Dispose();

                Should.Throw(() => dbContext.Foos.First(), typeof(ObjectDisposedException));
            }
        }
    }
}