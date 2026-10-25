using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistance;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasMany(r => r.Tables)
            .WithMany(t => t.Reservations);

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin"
            },
            new Role
            {
                Id = 2,
                Name = "User"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Juan",
                Surname = "Perez",
                Password = "123456",
                Email = "juan@example.com",
                PhoneNumber = "3411111111",
                RoleId = 1,
            },
            new User
            {
                Id = 2,
                Name = "Maria",
                Surname = "Gomez",
                Password = "123456",
                Email = "maria@example.com",
                PhoneNumber = "3412222222",
                RoleId = 2
            },
            new User
            {
                Id = 3,
                Name = "Pedro",
                Surname = "Lopez",
                Password = "123456",
                Email = "pedro@example.com",
                PhoneNumber = "3413333333",
                RoleId = 2
            },
            new User
            {
                Id = 4,
                Name = "Ana",
                Surname = "Martinez",
                Password = "123456",
                Email = "ana@example.com",
                PhoneNumber = "3414444444",
                RoleId = 2
            },
            new User
            {
                Id = 5,
                Name = "Lucas",
                Surname = "Fernandez",
                Password = "123456",
                Email = "lucas@example.com",
                PhoneNumber = "3415555555",
                RoleId = 2
            }
        );
    }
}
