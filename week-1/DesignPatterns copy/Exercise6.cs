using System;

interface IImage
{
    void Display();
}

class RealImage : IImage
{
    private string fileName;

    public RealImage(string fileName)
    {
        this.fileName = fileName;
        LoadFromServer();
    }

    private void LoadFromServer()
    {
        Console.WriteLine("Loading image from remote server: " + fileName);
    }

    public void Display()
    {
        Console.WriteLine("Displaying image: " + fileName);
    }
}

class ProxyImage : IImage
{
    private RealImage realImage;
    private string fileName;

    public ProxyImage(string fileName)
    {
        this.fileName = fileName;
    }

    public void Display()
    {
        if (realImage == null)
        {
            realImage = new RealImage(fileName);
        }

        realImage.Display();
    }
}

class Program
{
    static void Main()
    {
        IImage image = new ProxyImage("nature.jpg");

        image.Display();
        Console.WriteLine();

        image.Display();
    }
}