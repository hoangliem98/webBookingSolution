using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class ServiceInBookConfiguration : IEntityTypeConfiguration<ServiceInBook>
    {
        public void Configure(EntityTypeBuilder<ServiceInBook> builder)
        {
            builder.HasKey(t => new { t.BookId, t.ServiceId });
            builder.ToTable("ServiceInBooks");

            builder.HasOne(t => t.Service).WithMany(sb => sb.ServiceInBooks).HasForeignKey(sb=>sb.ServiceId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(t => t.Book).WithMany(sb => sb.ServiceInBooks).HasForeignKey(sb => sb.BookId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
