using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crudify.EntityFrameworkCore
{
    public class DbContextCrudRepositoryOptions
    {
        public DbContext DbContext { get; set; }
        public ILogger Logger { get; set; }
        public bool DisposeOfDbContextOnDispose { get; set; } = true;
    }
}