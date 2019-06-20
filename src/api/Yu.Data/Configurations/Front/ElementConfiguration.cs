using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yu.Data.Entities.Enums;
using Yu.Data.Entities.Front;

namespace Yu.Data.Configurations.Front
{
    public class ElementConfiguration : IEntityTypeConfiguration<Element>
    {
        public void Configure(EntityTypeBuilder<Element> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.ElementType).HasConversion(et => (int)et, et => (ElementType)et);
        }
    }
}
