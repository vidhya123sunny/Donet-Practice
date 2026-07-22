using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Filters;
using EmployeeManagement.Api.Mapping;
using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,POC")]
[ServiceFilter<CustomExceptionFilter>]
public sealed class EmployeesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet(Name = "GetEmployees")]
    [ProducesResponseType<IReadOnlyCollection<EmployeeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<EmployeeDto>>> Get([FromQuery] bool throwError = false)
    {
        if (throwError)
        {
            throw new InvalidOperationException("Exception filter demonstration from GET employees.");
        }

        var employees = await dbContext.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Skills)
            .OrderBy(employee => employee.Id)
            .ToListAsync();

        return Ok(employees.Select(employee => employee.ToDto()).ToArray());
    }

    [HttpGet("{id:int}", Name = "GetEmployeeById")]
    [ProducesResponseType<EmployeeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await FindEmployeeAsync(id);

        return employee is null ? NotFound() : Ok(employee.ToDto());
    }

    [HttpPost]
    [ProducesResponseType<EmployeeDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] UpsertEmployeeRequest request)
    {
        var validationResult = await ValidateRequestAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var employee = new Employee();
        await ApplyRequestAsync(employee, request);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(employee).Reference(item => item.Department).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee.ToDto());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<EmployeeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpsertEmployeeRequest request)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid employee id");
        }

        var employee = await FindEmployeeAsync(id);
        if (employee is null)
        {
            return BadRequest("Invalid employee id");
        }

        var validationResult = await ValidateRequestAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await ApplyRequestAsync(employee, request);
        await dbContext.SaveChangesAsync();

        return Ok(employee.ToDto());
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid employee id");
        }

        var employee = await dbContext.Employees.FindAsync(id);
        if (employee is null)
        {
            return BadRequest("Invalid employee id");
        }

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<Employee?> FindEmployeeAsync(int id)
    {
        return await dbContext.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.Skills)
            .FirstOrDefaultAsync(employee => employee.Id == id);
    }

    private async Task<ActionResult?> ValidateRequestAsync(UpsertEmployeeRequest request)
    {
        if (!await dbContext.Departments.AnyAsync(department => department.Id == request.DepartmentId))
        {
            return BadRequest("Invalid department id");
        }

        var distinctSkillIds = request.SkillIds.Distinct().ToArray();
        var existingSkillCount = await dbContext.Skills.CountAsync(skill => distinctSkillIds.Contains(skill.Id));

        return existingSkillCount == distinctSkillIds.Length
            ? null
            : BadRequest("Invalid skill id");
    }

    private async Task ApplyRequestAsync(Employee employee, UpsertEmployeeRequest request)
    {
        var skillIds = request.SkillIds.Distinct().ToArray();
        var skills = await dbContext.Skills
            .Where(skill => skillIds.Contains(skill.Id))
            .OrderBy(skill => skill.Id)
            .ToListAsync();

        employee.Name = request.Name;
        employee.Salary = request.Salary;
        employee.Permanent = request.Permanent;
        employee.DepartmentId = request.DepartmentId;
        employee.DateOfBirth = request.DateOfBirth;
        employee.Skills = skills;
    }
}
