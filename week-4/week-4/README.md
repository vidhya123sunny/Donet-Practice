# Week 4 - Microservices with JWT

This week contains two independent ASP.NET Core microservices:

- `AuthService` issues JWT bearer tokens.
- `EmployeeService` validates those JWTs before exposing employee APIs.

## Run

Open two terminals from `DeepSkilling/week-4`.

```bash
dotnet run --project AuthService/AuthService.csproj
dotnet run --project EmployeeService/EmployeeService.csproj
```

Swagger endpoints:

- Auth service: `http://localhost:5101/swagger`
- Employee service: `http://localhost:5102/swagger`

## Demo Users

| User name | Password | Role |
| --- | --- | --- |
| `admin` | `admin@123` | `Admin` |
| `poc` | `poc@123` | `POC` |
| `associate` | `associate@123` | `Associate` |

## Test Flow

1. Generate a token from `AuthService` using `GET /api/auth/token?userName=admin` or `POST /api/auth/login`.
2. Copy the `accessToken` value.
3. Open `EmployeeService` Swagger, click Authorize, and enter `Bearer {accessToken}`.
4. Call `GET /api/employees` or `POST /api/employees`.

The `.http` file has the same calls in REST Client format.
