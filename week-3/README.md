# Week 3 - ASP.NET Core Web API

This folder contains the Week 3 Web API hands-on implementation.

## Project

- `EmployeeManagement.Api` - ASP.NET Core 8 Web API using Entity Framework Core with SQLite.

## Covered Concepts

- REST API controllers with `GET`, `POST`, `PUT`, and `DELETE`.
- Swagger/OpenAPI with response metadata and JWT authorization support.
- Custom `Employee`, `Department`, and `Skill` model classes.
- Entity Framework Core `DbContext`, relationships, SQLite persistence, and seed data.
- `[FromBody]` request binding through create/update DTOs.
- Custom authorization filter and custom exception filter classes.
- CORS policy for local frontend clients.
- JWT bearer authentication with role-based authorization.

## Run

```bash
cd DeepSkilling/week-3
dotnet restore
dotnet run --project EmployeeManagement.Api/EmployeeManagement.Api.csproj
```

Swagger opens at:

```text
https://localhost:7201/swagger
http://localhost:5208/swagger
```

## Auth Flow

Generate a token:

```http
GET /api/auth/token?userId=1&role=Admin
```

Use the returned token in Swagger or Postman:

```text
Authorization: Bearer <token>
```

Protected employee endpoints:

```text
GET    /api/employees
GET    /api/employees/{id}
POST   /api/employees
PUT    /api/employees/{id}
DELETE /api/employees/{id}
```

To test role rejection, generate a token with `role=User` and call `GET /api/employees`; the API returns `403 Forbidden`. To test expiration, reduce the token lifetime in `AuthController`.

## Notes

- The SQLite database is created automatically as `EmployeeManagement.Api/employee-management.db`.
- `GET /api/employees?throwError=true` demonstrates the custom exception filter and writes details to `EmployeeManagement.Api/Logs/exceptions.log`.
- `CustomAuthFilter` is included to match the handson exercise, but the controller uses the production-style `[Authorize]` JWT flow from the later exercise.
