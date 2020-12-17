using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.ToTable("Halls");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().IsUnicode();
            builder.Property(x => x.Description).IsRequired().IsUnicode();
            builder.Property(x => x.MinimumTables).IsRequired();
            builder.Property(x => x.MaximumTables).IsRequired();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}
