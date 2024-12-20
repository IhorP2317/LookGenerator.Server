using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class AttributeType:BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<AttributeOption> AttributeOptions { get; set; } = new List<AttributeOption>();
    }