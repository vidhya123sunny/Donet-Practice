using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.Api.Filters;

public sealed class CustomExceptionFilter(IHostEnvironment environment) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var logsDirectory = Path.Combine(environment.ContentRootPath, "Logs");
        Directory.CreateDirectory(logsDirectory);

        var logEntry = $"{DateTimeOffset.UtcNow:u} {context.Exception}\n";
        File.AppendAllText(Path.Combine(logsDirectory, "exceptions.log"), logEntry);

        context.Result = new ObjectResult("Internal server error. Check Logs/exceptions.log for details.")
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
