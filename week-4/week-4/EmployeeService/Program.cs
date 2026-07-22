using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetRequiredSection("Jwt");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = securityKey,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeReaders", policy =>
        policy.RequireRole("Admin", "POC", "Associate"));
    options.AddPolicy("EmployeeWriters", policy =>
        policy.RequireRole("Admin", "POC"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Week 4 Employee Microservice",
        Version = "v1",
        Description = "Employee endpoints protected by JWT bearer authentication."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste a JWT from AuthService as: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/employees", [Authorize(Policy = "EmployeeReaders")] () =>
{
    return Results.Ok(EmployeeStore.All);
})
.WithName("GetEmployees")
.WithOpenApi();

app.MapGet("/api/employees/{id:int}", [Authorize(Policy = "EmployeeReaders")] (int id) =>
{
    var employee = EmployeeStore.All.FirstOrDefault(item => item.Id == id);
    return employee is null ? Results.NotFound() : Results.Ok(employee);
})
.WithName("GetEmployeeById")
.WithOpenApi();

app.MapPost("/api/employees", [Authorize(Policy = "EmployeeWriters")] (UpsertEmployeeRequest request) =>
{
    var employee = EmployeeStore.Add(request);
    return Results.Created($"/api/employees/{employee.Id}", employee);
})
.WithName("CreateEmployee")
.WithOpenApi();

app.MapGet("/api/profile", [Authorize] (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        Name = user.Identity?.Name,
        Role = user.FindFirstValue(ClaimTypes.Role),
        UserId = user.FindFirstValue("userId")
    });
})
.WithName("GetAuthenticatedProfile")
.WithOpenApi();

app.Run();

internal sealed record EmployeeDto(
    int Id,
    string Name,
    string Department,
    string Designation,
    int Salary,
    bool Permanent);

internal sealed record UpsertEmployeeRequest(
    string Name,
    string Department,
    string Designation,
    int Salary,
    bool Permanent);

internal static class EmployeeStore
{
    private static readonly List<EmployeeDto> Employees =
    [
        new(1, "John Doe", "Engineering", "Senior Developer", 65000, true),
        new(2, "Jane Smith", "Quality Assurance", "QA Lead", 58000, true),
        new(3, "Robert Brown", "Delivery", "Project Manager", 76000, false)
    ];

    public static IReadOnlyCollection<EmployeeDto> All => Employees;

    public static EmployeeDto Add(UpsertEmployeeRequest request)
    {
        var nextId = Employees.Count == 0 ? 1 : Employees.Max(item => item.Id) + 1;
        var employee = new EmployeeDto(
            nextId,
            request.Name,
            request.Department,
            request.Designation,
            request.Salary,
            request.Permanent);

        Employees.Add(employee);
        return employee;
    }
}
