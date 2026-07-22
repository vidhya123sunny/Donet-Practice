using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Mapping;

public static class EmployeeMapping
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        var department = employee.Department is null
            ? new DepartmentDto(employee.DepartmentId, string.Empty)
            : new DepartmentDto(employee.Department.Id, employee.Department.Name);

        return new EmployeeDto(
            employee.Id,
            employee.Name,
            employee.Salary,
            employee.Permanent,
            department,
            employee.Skills
                .OrderBy(skill => skill.Name)
                .Select(skill => new SkillDto(skill.Id, skill.Name))
                .ToArray(),
            employee.DateOfBirth);
    }
}
