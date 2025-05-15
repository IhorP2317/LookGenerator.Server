using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities ;

    public class User:BaseEntity
    {
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool EmailConfirmed { get; set; } 
        public ICollection<Look> Looks { get; set; } = new List<Look>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
       
        public ICollection<AttributeOption> AttributeOptions { get; set; } = new List<AttributeOption>();
      
        public ICollection<AttributeType> AttributeTypes { get; set; } = new List<AttributeType>();
        
        public ICollection<MasterSizeIdentifier> MasterSizeIdentifiers { get; set; } = new List<MasterSizeIdentifier>();
       
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
            
        public ICollection<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();
        
        public ICollection<ProductVariation> ProductVariations { get; set; } = new List<ProductVariation>();
       
        public ICollection<SizeCategory> SizeCategories { get; set; } = new List<SizeCategory>();
        
        public ICollection<SizeOption> SizeOptions { get; set; } = new List<SizeOption>();
       
        public ICollection<User> Users { get; set; } = new List<User>();
    }