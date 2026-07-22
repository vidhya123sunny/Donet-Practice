using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Week 4 Auth Microservice",
        Version = "v1",
        Description = "Issues JWT bearer tokens for the employee microservice."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapPost("/api/auth/login", ([FromBody] LoginRequest request, IConfiguration configuration) =>
{
    var user = DemoUsers.Find(request.UserName, request.Password);
    if (user is null)
    {
        return Results.Unauthorized();
    }

    var token = JwtTokenFactory.Create(user, configuration);
    return Results.Ok(token);
})
.WithName("Login")
.WithOpenApi();

app.MapGet("/api/auth/token", (IConfiguration configuration, string userName = "admin") =>
{
    var user = DemoUsers.FindByUserName(userName) ?? DemoUsers.Admin;
    var token = JwtTokenFactory.Create(user, configuration);
    return Results.Ok(token);
})
.WithName("DemoToken")
.WithOpenApi();

app.Run();

internal sealed record LoginRequest(string UserName, string Password);

internal sealed record AuthenticatedUser(int Id, string UserName, string DisplayName, string Role);

internal sealed record TokenResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAt,
    string UserName,
    string Role);

internal static class DemoUsers
{
    public static readonly AuthenticatedUser Admin = new(1, "admin", "System Administrator", "Admin");

    private static readonly IReadOnlyCollection<(AuthenticatedUser User, string Password)> Users =
    [
        (Admin, "admin@123"),
        (new AuthenticatedUser(2, "poc", "Project Coordinator", "POC"), "poc@123"),
        (new AuthenticatedUser(3, "associate", "Associate User", "Associate"), "associate@123")
    ];

    public static AuthenticatedUser? Find(string userName, string password)
    {
        return Users.FirstOrDefault(item =>
            string.Equals(item.User.UserName, userName, StringComparison.OrdinalIgnoreCase) &&
            item.Password == password).User;
    }

    public static AuthenticatedUser? FindByUserName(string userName)
    {
        return Users.FirstOrDefault(item =>
            string.Equals(item.User.UserName, userName, StringComparison.OrdinalIgnoreCase)).User;
    }
}

internal static class JwtTokenFactory
{
    public static TokenResponse Create(AuthenticatedUser user, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetRequiredSection("Jwt");
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue("ExpiryMinutes", 30));
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Role, user.Role),
            new("userId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expiresAt,
            user.UserName,
            user.Role);
    }
}
