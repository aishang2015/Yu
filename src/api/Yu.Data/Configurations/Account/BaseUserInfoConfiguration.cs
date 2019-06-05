using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Yu.Data.Entities.Account;
using Yu.Data.Entities.Enums;

namespace Yu.Data.Configurations.Account
{
    public class BaseUserInfoConfiguration : IEntityTypeConfiguration<BaseUserInfo>
    {
        public void Configure(EntityTypeBuilder<BaseUserInfo> builder)
        {
            // 作为user的附表 主键设为主表的主键
            builder.HasKey(baseUserInfo=> baseUserInfo.Id);
            builder.Property(baseUserInfo => baseUserInfo.Id).ValueGeneratedNever();
            builder.Property(baseUserInfo => baseUserInfo.Gender)
                .HasConversion(v => (int)v, v => (Gender)v)
                .HasDefaultValue(Gender.未知);
        }
    }
}
