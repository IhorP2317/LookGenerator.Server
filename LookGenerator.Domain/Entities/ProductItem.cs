using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class ProductItem:BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public Colour Colour { get; set; }
        public ICollection<ProductImage> Images = new List<ProductImage>();
        public ICollection<ProductVariation> Variations = new List<ProductVariation>();
    }