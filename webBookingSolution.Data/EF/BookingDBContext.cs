using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.Data.Configurations;
using webBookingSolution.Data.Entities;
namespace webBookingSolution.Data.EF
{
    public class BookingDBContext : DbContext
    {
        public BookingDBContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new HallConfiguration());
            modelBuilder.ApplyConfiguration(new MenuConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceInBookConfiguration());
            modelBuilder.ApplyConfiguration(new HallImageConfiguration());
            //base.OnModelCreating(modelBuilder);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ServiceInBook> ServiceInBooks { get; set; }
        public DbSet<HallImage> HallImages { get; set; }
    }
}
