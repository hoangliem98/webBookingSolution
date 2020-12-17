using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class HallImageConfiguration : IEntityTypeConfiguration<HallImage>
    {
        public void Configure(EntityTypeBuilder<HallImage> builder)
        {
            builder.ToTable("HallImages");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Caption).HasMaxLength(200);
            builder.Property(x => x.Path).HasMaxLength(200).IsRequired();
            builder.Property(x => x.DateCreated).IsRequired().HasDefaultValue(DateTime.Now);

            builder.HasOne(x => x.Hall).WithMany(x => x.HallImages).HasForeignKey(x => x.HallId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
