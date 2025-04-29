using LookGenerator.Domain.Common;

namespace LookGenerator.Domain.Entities;

public class ProductLink:BaseEntity
{
    public string Url{ get; set; } = default!;
    public string RegionName { get; set; }= default!;
    public Guid ProductItemId { get; set; }
    public ProductItem ProductItem { get; set; } = default!;
}