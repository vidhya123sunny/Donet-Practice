using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Employees.AnyAsync())
        {
            return;
        }

        var skills = await dbContext.Skills.ToDictionaryAsync(skill => skill.Id);

        dbContext.Employees.AddRange(
            new Employee
            {
                Name = "John Doe",
                Salary = 50000,
                Permanent = true,
                DepartmentId = 1,
                DateOfBirth = new DateTime(1990, 5, 12),
                Skills = [skills[1], skills[2], skills[3]]
            },
            new Employee
            {
                Name = "Jane Smith",
                Salary = 65000,
                Permanent = false,
                DepartmentId = 2,
                DateOfBirth = new DateTime(1988, 8, 25),
                Skills = [skills[1], skills[4]]
            },
            new Employee
            {
                Name = "Robert Brown",
                Salary = 72000,
                Permanent = true,
                DepartmentId = 3,
                DateOfBirth = new DateTime(1985, 11, 4),
                Skills = [skills[2], skills[3], skills[5]]
            });

        await dbContext.SaveChangesAsync();
    }
}
