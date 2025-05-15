using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class Product:BaseEntity
    {
        public string Name { get; set; } = default!;
        public long ExternalId { get; set; }
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public ProductBodyZone BodyZone { get; set; }
        public string Gender { get; set; } = string.Empty;
        public ProductCategory ProductCategory { get; set; } = default!;
        public ICollection<ProductItem> Items { get; set; } = new List<ProductItem>();
        public List<ProductAttributeOption> ProductAttributeOptions { get; set; } = [];
    }