namespace LookGenerator.Domain.Entities ;

    public class LookProductVariation
    {
        public Guid LookId { get; set; }
        public Look Look { get; set; } = default!;
        public Guid ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; } = default!;
    }