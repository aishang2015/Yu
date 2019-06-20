using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yu.Data.Entities.Enums;
using Yu.Data.Entities.Right;

namespace Yu.Data.Configurations.Right
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.PermissionType).HasConversion(pt => (int)pt, pt => (PermissionType)pt)
                .HasDefaultValue(PermissionType.未知);
        }
    }
}
