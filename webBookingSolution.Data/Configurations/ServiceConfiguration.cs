using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.ToTable("Services");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().IsUnicode();
            builder.Property(x => x.Image).IsRequired();
            builder.Property(x => x.Description).IsRequired().IsUnicode();
            builder.Property(x => x.Price).IsRequired();
        }
    }
}
