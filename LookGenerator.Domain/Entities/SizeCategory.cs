using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class SizeCategory:BaseEntity
    {
        public string Name { get; set; } = default!;
        public Guid? ParentCategoryId { get; set; }
        public SizeCategory? ParentCategory { get; set; }
        public ICollection<SizeCategory>? SubCategories { get; set; }
        public ICollection<SizeOption>? SizeOptions { get; set; }
    }