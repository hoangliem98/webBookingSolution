using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.Roles).IsRequired();

            builder.HasOne(a => a.Customer).WithOne(c => c.Account).HasForeignKey<Customer>(c => c.AccountId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(a => a.Employee).WithOne(e => e.Account).HasForeignKey<Employee>(e => e.AccountId).OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
