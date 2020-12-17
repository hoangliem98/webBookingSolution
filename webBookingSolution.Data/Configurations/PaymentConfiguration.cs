using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.PaymentDate).IsRequired();
            builder.Property(x => x.TempPrice).IsRequired();
            builder.Property(x => x.DelayPrice).IsRequired();
            builder.Property(x => x.DelayContent).IsRequired(false).IsUnicode();
            builder.Property(x => x.TotalPrice).IsRequired();

            builder.HasKey(x => new { x.BookId, x.Id });
            builder.HasOne(x => x.Book).WithMany(x => x.Payments).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Employee).WithMany(x => x.Payments).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
