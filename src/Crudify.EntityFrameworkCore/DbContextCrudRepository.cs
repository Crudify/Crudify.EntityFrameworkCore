using Crudify.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Crudify.EntityFrameworkCore
{
    public class DbContextCrudRepository<TCrudEntity, TCrudEntityId> : ICrudRepository<TCrudEntity, TCrudEntityId>, IDisposable
            where TCrudEntity : class, ICrudEntity<TCrudEntityId>
            where TCrudEntityId : struct
    {
        public readonly DbContext DbContext;
        public readonly ILogger Logger;
        public bool KeepDbContextInitialized { get; set; } = true;

        public DbContextCrudRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbContextCrudRepository(DbContext dbContext, ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public DbContextCrudRepository(DbContext dbContext, ILogger logger, bool keepDbContextInitialized)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            KeepDbContextInitialized = keepDbContextInitialized;
        }

        public DbContextCrudRepository(DbContextCrudRepositoryOptions dbContextCrudRepositoryOptions)
        {
            if (dbContextCrudRepositoryOptions == null)
                throw new ArgumentNullException(nameof(dbContextCrudRepositoryOptions));

            DbContext = dbContextCrudRepositoryOptions.DbContext;
            Logger = dbContextCrudRepositoryOptions.Logger;
            KeepDbContextInitialized = dbContextCrudRepositoryOptions.DisposeOfDbContextOnDispose;
        }

        public TCrudEntityId Create(TCrudEntity crudEntity)
        {
            DbContext.Set<TCrudEntity>().Add(crudEntity);
            DbContext.SaveChanges(true);
            DbContext.Entry(crudEntity).State = EntityState.Detached;
            return crudEntity.Id;
        }

        public async Task<TCrudEntityId> CreateAsync(TCrudEntity crudEntity)
        {
            DbContext.Set<TCrudEntity>().Add(crudEntity);
            await DbContext.SaveChangesAsync(true);
            DbContext.Entry(crudEntity).State = EntityState.Detached;
            return crudEntity.Id;
        }

        public TCrudEntity Read(TCrudEntityId crudEntityId)
        {
            return DbContext.Set<TCrudEntity>().AsNoTracking().FirstOrDefault(e => e.Id.Equals(crudEntityId));
        }

        public async Task<TCrudEntity> ReadAsync(TCrudEntityId crudEntityId)
        {
            return await DbContext.Set<TCrudEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id.Equals(crudEntityId));
        }

        public void Update(TCrudEntity crudEntity)
        {
            DbContext.Set<TCrudEntity>().Update(crudEntity);
            DbContext.SaveChanges();
            DbContext.Entry(crudEntity).State = EntityState.Detached;
        }

        public async Task UpdateAsync(TCrudEntity crudEntity)
        {
            DbContext.Set<TCrudEntity>().Update(crudEntity);
            await DbContext.SaveChangesAsync();
            DbContext.Entry(crudEntity).State = EntityState.Detached;
        }

        public void Delete(TCrudEntityId crudEntityId)
        {
            var model = Read(crudEntityId);
            DbContext.Set<TCrudEntity>().Remove(model);
            DbContext.SaveChanges();
            DbContext.Entry(model).State = EntityState.Detached;
        }

        public async Task DeleteAsync(TCrudEntityId crudEntityId)
        {
            var model = await ReadAsync(crudEntityId);
            DbContext.Set<TCrudEntity>().Remove(model);
            await DbContext.SaveChangesAsync();
            DbContext.Entry(model).State = EntityState.Detached;
        }

        public void Dispose()
        {
            if (!KeepDbContextInitialized)
            {
                DbContext.Dispose();
            }
        }
    }
}