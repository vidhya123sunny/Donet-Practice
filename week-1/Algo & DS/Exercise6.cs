// Exercise 6: Library Management System

using System;

class Book
{
    public int BookId;
    public string Title;
    public string Author;

    public Book(int id, string title, string author)
    {
        BookId = id;
        Title = title;
        Author = author;
    }
}

class Program
{
    static int LinearSearch(Book[] books, string title)
    {
        for (int i = 0; i < books.Length; i++)
        {
            if (books[i].Title == title)
                return i;
        }

        return -1;
    }

    static int BinarySearch(Book[] books, string title)
    {
        int left = 0;
        int right = books.Length - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;

            int compare = string.Compare(books[mid].Title, title);

            if (compare == 0)
                return mid;

            if (compare < 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }

    static void Main()
    {
        Book[] books =
        {
            new Book(1,"CSharp","Author1"),
            new Book(2,"DBMS","Author2"),
            new Book(3,"Java","Author3"),
            new Book(4,"Python","Author4")
        };

        Console.WriteLine("Linear Search Index: " +
                          LinearSearch(books, "Java"));

        Console.WriteLine("Binary Search Index: " +
                          BinarySearch(books, "Java"));
    }
}