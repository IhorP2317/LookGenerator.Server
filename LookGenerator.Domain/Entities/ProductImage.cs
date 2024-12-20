using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class ProductImage:BaseEntity
    {
        public string ImageUrl { get; set; } = default!;
        public bool ProductOnly { get; set; }
        public Guid ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; } = default!;
    }