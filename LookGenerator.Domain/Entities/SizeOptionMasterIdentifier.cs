using System.Reflection.Metadata.Ecma335;

namespace LookGenerator.Domain.Entities;

public class SizeOptionMasterIdentifier
{
    public Guid SizeOptionId { get; set; }
    public SizeOption SizeOption { get; set; } = default!;
    
    public Guid MasterIdentifierId { get; set; }
    public MasterSizeIdentifier MasterSizeIdentifier { get; set; } = default!;
    public string?  Name { get; set; }
    
}