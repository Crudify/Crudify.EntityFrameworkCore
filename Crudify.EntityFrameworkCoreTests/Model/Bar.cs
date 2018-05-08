using Crudify.Core.Interfaces;

namespace Crudify.EntityFrameworkCoreTests.Model
{
    public class Bar : ICrudEntity<ulong>
    {
        public ulong Id { get; set; }
        public string Stool { get; set; }
    }
}