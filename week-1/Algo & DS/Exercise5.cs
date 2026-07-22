// Exercise 5: Task Management System

using System;

class TaskNode
{
    public int TaskId;
    public string TaskName;
    public string Status;
    public TaskNode Next;

    public TaskNode(int id, string name, string status)
    {
        TaskId = id;
        TaskName = name;
        Status = status;
        Next = null;
    }
}

class TaskLinkedList
{
    private TaskNode head;

    public void AddTask(int id, string name, string status)
    {
        TaskNode newNode = new TaskNode(id, name, status);

        if (head == null)
        {
            head = newNode;
            return;
        }

        TaskNode temp = head;

        while (temp.Next != null)
        {
            temp = temp.Next;
        }

        temp.Next = newNode;
    }

    public void SearchTask(int id)
    {
        TaskNode temp = head;

        while (temp != null)
        {
            if (temp.TaskId == id)
            {
                Console.WriteLine($"Task Found: {temp.TaskName}");
                return;
            }
            temp = temp.Next;
        }

        Console.WriteLine("Task Not Found");
    }

    public void DeleteTask(int id)
    {
        if (head == null)
            return;

        if (head.TaskId == id)
        {
            head = head.Next;
            return;
        }

        TaskNode temp = head;

        while (temp.Next != null && temp.Next.TaskId != id)
        {
            temp = temp.Next;
        }

        if (temp.Next != null)
        {
            temp.Next = temp.Next.Next;
        }
    }

    public void DisplayTasks()
    {
        TaskNode temp = head;

        while (temp != null)
        {
            Console.WriteLine($"{temp.TaskId} {temp.TaskName} {temp.Status}");
            temp = temp.Next;
        }
    }
}

class Program
{
    static void Main()
    {
        TaskLinkedList tasks = new TaskLinkedList();

        tasks.AddTask(1, "Design", "Pending");
        tasks.AddTask(2, "Coding", "In Progress");

        tasks.DisplayTasks();

        tasks.SearchTask(2);

        tasks.DeleteTask(1);

        Console.WriteLine("\nAfter Deletion");

        tasks.DisplayTasks();
    }
}