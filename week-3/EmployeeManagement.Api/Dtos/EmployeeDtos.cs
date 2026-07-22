using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Dtos;

public sealed record SkillDto(int Id, string Name);

public sealed record DepartmentDto(int Id, string Name);

public sealed record EmployeeDto(
    int Id,
    string Name,
    int Salary,
    bool Permanent,
    DepartmentDto Department,
    IReadOnlyCollection<SkillDto> Skills,
    DateTime DateOfBirth);

public sealed record UpsertEmployeeRequest(
    [Required] string Name,
    [Range(1, int.MaxValue)] int Salary,
    bool Permanent,
    [Range(1, int.MaxValue)] int DepartmentId,
    IReadOnlyCollection<int> SkillIds,
    DateTime DateOfBirth);

public sealed record TokenRequest(int UserId, string Role);

public sealed record TokenResponse(string Token, DateTime ExpiresAt);
