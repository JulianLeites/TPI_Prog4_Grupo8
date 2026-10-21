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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Juan",
                Surname = "Perez",
                Password = "123456",
                Email = "juan@example.com",
                PhoneNumber = "3411111111"
            },
            new User
            {
                Id = 2,
                Name = "Maria",
                Surname = "Gomez",
                Password = "123456",
                Email = "maria@example.com",
                PhoneNumber = "3412222222"
            },
            new User
            {
                Id = 3,
                Name = "Pedro",
                Surname = "Lopez",
                Password = "123456",
                Email = "pedro@example.com",
                PhoneNumber = "3413333333"
            },
            new User
            {
                Id = 4,
                Name = "Ana",
                Surname = "Martinez",
                Password = "123456",
                Email = "ana@example.com",
                PhoneNumber = "3414444444"
            },
            new User
            {
                Id = 5,
                Name = "Lucas",
                Surname = "Fernandez",
                Password = "123456",
                Email = "lucas@example.com",
                PhoneNumber = "3415555555"
            }
        );
    }
}
