using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class ProductVariation:BaseEntity
    {
        public Guid ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; } = default!;
        public decimal Price { get; set; }
        public string Size { get; set; } = default!;
        public string SizeType { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
        public Guid MasterSizeIdentifierId { get; set; }
        public MasterSizeIdentifier MasterSizeIdentifier { get; set; } = default!;
        public ICollection<LookProductVariation> LookProductVariations { get; set; } = new List<LookProductVariation>();
    }