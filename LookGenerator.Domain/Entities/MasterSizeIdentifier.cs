using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class MasterSizeIdentifier
    {
        public string Identifier { get; set; } = default!;
        public Guid SizeOptionId { get; set; } 
        public SizeOption SizeOption { get; set; } = default!;
        public ICollection<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();
    }