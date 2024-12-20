using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class AttributeOption:BaseEntity
    {
        public string Name { get; set; } = default!;
        public Guid AttributeTypeId { get; set; }
        public AttributeType AttributeType { get; set; } = default!;
        public ICollection<ProductAttributeOption> ProductAttributeOptions { get; set; } = new List<ProductAttributeOption>();
    }