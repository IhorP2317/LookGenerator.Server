using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class Product:BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = default!;
        public ICollection<ProductItem> Items { get; set; } = new List<ProductItem>();
        public ICollection<ProductAttributeOption> ProductAttributeOptions { get; set; } = new List<ProductAttributeOption>();
    }