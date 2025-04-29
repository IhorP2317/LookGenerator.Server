namespace LookGenerator.Domain.Entities;

public class Colour
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}