using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class Look:BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<LookProductVariation> LookProductVariations { get; set; } = new List<LookProductVariation>();
        public User User { get; set; } = default!;
    }