using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yu.Core.Expressions;
using Yu.Data.Entities.Right;

namespace Yu.Data.Configurations.Right
{
    public class RuleConditionConfiguration : IEntityTypeConfiguration<RuleCondition>
    {
        public void Configure(EntityTypeBuilder<RuleCondition> builder)
        {
            builder.HasKey(rc => rc.Id);
            builder.Property(rc => rc.Id).ValueGeneratedOnAdd();

            builder.Property(rc => rc.OperateType).HasConversion(o => (int)o, o => (ExpressionType)o);
        }
    }
}
