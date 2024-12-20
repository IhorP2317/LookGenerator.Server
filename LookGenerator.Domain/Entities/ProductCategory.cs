using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class ProductCategory: BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public ICollection<Product>? Products { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public ProductCategory? ParentCategory { get; set; }
        public ICollection<ProductCategory>? SubCategories { get; set; }
        public Guid? SizeCategoryId { get; set; } 
        public SizeCategory? SizeCategory { get; set; }
    }