using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yu.Core.Expressions;
using Yu.Data.Entities.Right;

namespace Yu.Data.Configurations.Right
{
    public class RuleConfiguration : IEntityTypeConfiguration<Rule>
    {
        public void Configure(EntityTypeBuilder<Rule> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever();
            builder.Property(r => r.CombineType).HasConversion(c => (int)c, c => (ExpressionCombineType)c);
        }
    }
}
