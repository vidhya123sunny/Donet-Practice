// Exercise 4: Employee Management System

using System;

class Employee
{
    public int EmployeeId;
    public string Name;
    public string Position;
    public double Salary;

    public Employee(int id, string name, string position, double salary)
    {
        EmployeeId = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}

class EmployeeManager
{
    private Employee[] employees = new Employee[10];
    private int count = 0;

    public void AddEmployee(Employee employee)
    {
        if (count < employees.Length)
        {
            employees[count++] = employee;
        }
    }

    public Employee SearchEmployee(int id)
    {
        for (int i = 0; i < count; i++)
        {
            if (employees[i].EmployeeId == id)
                return employees[i];
        }
        return null;
    }

    public void DeleteEmployee(int id)
    {
        for (int i = 0; i < count; i++)
        {
            if (employees[i].EmployeeId == id)
            {
                for (int j = i; j < count - 1; j++)
                {
                    employees[j] = employees[j + 1];
                }
                count--;
                break;
            }
        }
    }

    public void DisplayEmployees()
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{employees[i].EmployeeId} {employees[i].Name} {employees[i].Position} {employees[i].Salary}");
        }
    }
}

class Program
{
    static void Main()
    {
        EmployeeManager manager = new EmployeeManager();

        manager.AddEmployee(new Employee(101, "Prem", "Developer", 50000));
        manager.AddEmployee(new Employee(102, "Rahul", "Tester", 40000));

        manager.DisplayEmployees();

        Console.WriteLine("\nSearching Employee 101");

        Employee emp = manager.SearchEmployee(101);

        if (emp != null)
            Console.WriteLine(emp.Name);

        manager.DeleteEmployee(102);

        Console.WriteLine("\nAfter Deletion");

        manager.DisplayEmployees();
    }
}