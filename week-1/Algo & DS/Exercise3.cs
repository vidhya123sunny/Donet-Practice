//Sorting Customer Orders
using System;

class Order
{
    public int OrderId;
    public string CustomerName;
    public double TotalPrice;

    public Order(int id, string name, double price)
    {
        OrderId = id;
        CustomerName = name;
        TotalPrice = price;
    }
}

class Program
{
    static void BubbleSort(Order[] orders)
    {
        int n = orders.Length;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (orders[j].TotalPrice > orders[j + 1].TotalPrice)
                {
                    Order temp = orders[j];
                    orders[j] = orders[j + 1];
                    orders[j + 1] = temp;
                }
            }
        }
    }

    static void PrintOrders(Order[] orders)
    {
        foreach (var order in orders)
        {
            Console.WriteLine($"{order.OrderId} {order.CustomerName} {order.TotalPrice}");
        }
    }

    static void Main()
    {
        Order[] orders =
        {
            new Order(101,"Prem",5000),
            new Order(102,"Rahul",3000),
            new Order(103,"Anil",7000)
        };

        BubbleSort(orders);

        Console.WriteLine("Sorted Orders:");
        PrintOrders(orders);
    }
}