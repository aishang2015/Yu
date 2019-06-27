using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yu.Data.Entities.Right;

namespace Yu.Data.Configurations.Right
{
    public class RuleGroupConfiguration : IEntityTypeConfiguration<RuleGroup>
    {
        public void Configure(EntityTypeBuilder<RuleGroup> builder)
        {
            builder.HasKey(rg => rg.Id);
            builder.Property(rg => rg.Id).ValueGeneratedNever();
        }

    }
}
