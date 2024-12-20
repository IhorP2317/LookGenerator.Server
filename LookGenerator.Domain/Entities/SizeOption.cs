using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class SizeOption : BaseEntity
    {
        public string Name { get; set; } = default!;
        public double Cm { get; set; } 
        public double Inch { get; set; } 
        public Guid SizeCategoryId { get; set; } 
        public SizeCategory SizeCategory { get; set; } = default!;
        public ICollection<MasterSizeIdentifier> MasterSizeIds { get; set; } = new List<MasterSizeIdentifier>();
    }