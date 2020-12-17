using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.NumberTables).IsRequired();
            builder.Property(x => x.OrganizationDate).IsRequired();
            builder.Property(x => x.BookDate).IsRequired().HasDefaultValue(DateTime.Now);
            builder.Property(x => x.Price).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasDefaultValue("Chưa thanh toán");
            builder.Property(x => x.Season).IsRequired();

            //builder.HasKey(x => new { x.HallId, x.MenuId, x.CustomerId });
            builder.HasOne(x => x.Hall).WithMany(x => x.Books).HasForeignKey(x => x.HallId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Menu).WithMany(x => x.Books).HasForeignKey(x => x.MenuId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Customer).WithMany(x => x.Books).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
