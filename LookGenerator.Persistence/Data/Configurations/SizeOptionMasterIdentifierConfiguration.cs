using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LookGenerator.Persistence.Data.Configurations;

public class SizeOptionMasterIdentifierConfiguration: IEntityTypeConfiguration<SizeOptionMasterIdentifier>
{
    public void Configure(EntityTypeBuilder<SizeOptionMasterIdentifier> builder)
    {
        builder.HasKey(s => new { s.SizeOptionId, s.MasterIdentifierId });
       builder.HasOne(s => s.SizeOption)
           .WithMany(s => s.MasterSizeIdentifiers).HasForeignKey(s => s.SizeOptionId).OnDelete(DeleteBehavior.Cascade);
       builder.HasOne(s => s.MasterSizeIdentifier)
           .WithMany(m => m.SizeOptions).HasForeignKey(s => s.MasterIdentifierId).OnDelete(DeleteBehavior.Cascade);
    }
}