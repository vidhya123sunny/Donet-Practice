//E-Commerce Platform Search Function
using System;

class Product
{
    public int ProductId;
    public string ProductName;

    public Product(int id, string name)
    {
        ProductId = id;
        ProductName = name;
    }
}

class Program
{
    static int LinearSearch(Product[] products, string target)
    {
        for (int i = 0; i < products.Length; i++)
        {
            if (products[i].ProductName == target)
                return i;
        }
        return -1;
    }

    static int BinarySearch(Product[] products, string target)
    {
        int left = 0;
        int right = products.Length - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;

            int compare = string.Compare(products[mid].ProductName, target);

            if (compare == 0)
                return mid;
            else if (compare < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }

    static void Main()
    {
        Product[] products =
        {
            new Product(1,"Keyboard"),
            new Product(2,"Laptop"),
            new Product(3,"Mouse"),
            new Product(4,"Printer")
        };

        Console.WriteLine("Linear Search Index: " +
                          LinearSearch(products, "Mouse"));

        Console.WriteLine("Binary Search Index: " +
                          BinarySearch(products, "Mouse"));
    }
}