using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class ProductItem:BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int ColourId { get; set; }
        public Colour Colour { get; set; } = default!;
        public ICollection<ProductImage> Images = new List<ProductImage>();
        public ICollection<ProductLink> Links = new List<ProductLink>();
        public ICollection<ProductVariation> Variations = new List<ProductVariation>();
    }
    