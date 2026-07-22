namespace EmployeeManagement.Api.Models;

public sealed class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Salary { get; set; }
    public bool Permanent { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public List<Skill> Skills { get; set; } = [];
    public DateTime DateOfBirth { get; set; }
}
