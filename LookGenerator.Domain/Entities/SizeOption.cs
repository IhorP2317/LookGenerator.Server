using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class SizeOption : BaseEntity
    {
        public double Cm { get; set; } 
        public double Inch { get; set; } 
        public Guid SizeCategoryId { get; set; } 
        public SizeCategory SizeCategory { get; set; } = default!;
        public ICollection<SizeOptionMasterIdentifier> MasterSizeIdentifiers { get; set; } = new List<SizeOptionMasterIdentifier>();
    }