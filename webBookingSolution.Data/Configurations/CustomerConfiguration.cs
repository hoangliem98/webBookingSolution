﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using webBookingSolution.Data.Entities;

namespace webBookingSolution.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.FirstName).IsRequired().IsUnicode();
            builder.Property(x => x.LastName).IsRequired().IsUnicode();
            builder.Property(x => x.DOB).IsRequired();
            builder.Property(x => x.Image).IsRequired().IsUnicode();
            builder.Property(x => x.Email).IsRequired().IsUnicode(false);
            builder.Property(x => x.Address).IsRequired().IsUnicode();
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(10);
        }
    }
}
