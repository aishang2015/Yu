using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Configurations
{
    public class TestConfigration : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.Property(test => test.Property1).HasColumnName("p1");
        }
    }
}
