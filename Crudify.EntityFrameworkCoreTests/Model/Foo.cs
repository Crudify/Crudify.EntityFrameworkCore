using Crudify.Core;

namespace Crudify.EntityFrameworkCoreTests.Model
{
    public class Foo : ICrudEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}