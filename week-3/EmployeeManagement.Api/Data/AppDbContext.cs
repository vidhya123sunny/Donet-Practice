using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasMany(employee => employee.Skills)
            .WithMany();

        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Department)
            .WithMany()
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering" },
            new Department { Id = 2, Name = "Human Resources" },
            new Department { Id = 3, Name = "Finance" });

        modelBuilder.Entity<Skill>().HasData(
            new Skill { Id = 1, Name = "C#" },
            new Skill { Id = 2, Name = "ASP.NET Core" },
            new Skill { Id = 3, Name = "Entity Framework Core" },
            new Skill { Id = 4, Name = "SQL" },
            new Skill { Id = 5, Name = "Azure" });
    }
}
