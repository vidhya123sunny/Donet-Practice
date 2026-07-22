using System;

interface ICustomerRepository
{
    string FindCustomerById(int id);
}

class CustomerRepositoryImpl : ICustomerRepository
{
    public string FindCustomerById(int id)
    {
        return "Customer Found with ID: " + id;
    }
}

class CustomerService
{
    private ICustomerRepository repository;

    public CustomerService(ICustomerRepository repository)
    {
        this.repository = repository;
    }

    public void GetCustomer(int id)
    {
        Console.WriteLine(repository.FindCustomerById(id));
    }
}

class Program
{
    static void Main()
    {
        ICustomerRepository repository =
            new CustomerRepositoryImpl();

        CustomerService service =
            new CustomerService(repository);

        service.GetCustomer(101);
    }
}