namespace LookGenerator.Domain.Entities ;

    public class ProductAttributeOption
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public Guid AttributeOptionId { get; set; }
        public AttributeOption AttributeOption { get; set; } = default!;
    }