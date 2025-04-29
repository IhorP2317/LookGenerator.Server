using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class MasterSizeIdentifier:BaseEntity
    {
        public string Identifier { get; set; } = default!;
        public ICollection<SizeOptionMasterIdentifier> SizeOptions { get; set; } = new List<SizeOptionMasterIdentifier>();
        public ICollection<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();
    }