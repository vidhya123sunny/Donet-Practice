using System.Text;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;

if (Assembly.GetEntryAssembly()?.GetName().Name is "ef" or "dotnet-ef")
{
    return;
}

var projectContentRoot = Path.Combine(Directory.GetCurrentDirectory(), "EmployeeManagement.Api");
var contentRoot = Directory.Exists(projectContentRoot)
    ? projectContentRoot
    : Directory.GetCurrentDirectory();
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:5208";

var host = new WebHostBuilder()
    .UseContentRoot(contentRoot)
    .UseKestrel()
    .UseUrls(urls)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var securityKey = context.Configuration["Jwt:SecurityKey"]!;
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var databasePath = Path.Combine(context.HostingEnvironment.ContentRootPath, "employee-management.db");

        services.AddControllers();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));
        services.AddScoped<CustomAuthFilter>();
        services.AddScoped<CustomExceptionFilter>();
        services.AddCors(options =>
        {
            options.AddPolicy("LocalClients", policy =>
                policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = context.Configuration["Jwt:Issuer"],
                    ValidAudience = context.Configuration["Jwt:Audience"],
                    IssuerSigningKey = symmetricSecurityKey
                };
            });
        services.AddAuthorization();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Week 3 Employee Web API",
                Version = "v1",
                Description = "Web API hands-on with Swagger, Entity Framework Core, CORS, filters, CRUD, and JWT authentication.",
                Contact = new OpenApiContact { Name = "John Doe", Email = "john@xyzmail.com", Url = new Uri("https://www.example.com") },
                License = new OpenApiLicense { Name = "License Terms", Url = new Uri("https://www.example.com") }
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
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
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    []
                }
            });
        });
    })
    .Configure(app =>
    {
        if (app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Week 3 Employee Web API");
            });
        }

        app.UseRouting();
        app.UseCors("LocalClients");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

await host.RunAsync();
