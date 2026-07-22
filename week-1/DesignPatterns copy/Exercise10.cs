using System;

class Student
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Grade { get; set; }
}

class StudentView
{
    public void DisplayStudentDetails(Student student)
    {
        Console.WriteLine("Student Details");
        Console.WriteLine("ID: " + student.Id);
        Console.WriteLine("Name: " + student.Name);
        Console.WriteLine("Grade: " + student.Grade);
    }
}

class StudentController
{
    private Student model;
    private StudentView view;

    public StudentController(Student model, StudentView view)
    {
        this.model = model;
        this.view = view;
    }

    public void SetStudentName(string name)
    {
        model.Name = name;
    }

    public void SetStudentGrade(string grade)
    {
        model.Grade = grade;
    }

    public void UpdateView()
    {
        view.DisplayStudentDetails(model);
    }
}

class Program
{
    static void Main()
    {
        Student student = new Student
        {
            Id = 101,
            Name = "Prem",
            Grade = "A"
        };

        StudentView view = new StudentView();
        StudentController controller =
            new StudentController(student, view);

        controller.UpdateView();

        Console.WriteLine();

        controller.SetStudentName("Pabbi");
        controller.SetStudentGrade("A+");

        controller.UpdateView();
    }
}