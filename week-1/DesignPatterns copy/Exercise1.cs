using System;
public sealed class Logger
{
    private static Logger instance;
    private Logger()
    {
        Console.WriteLine("Logger Instance Created");
    }
    public static Logger GetInstance()
    {
        if (instance == null)
        {
            instance = new Logger();
        }
        return instance;
    }
    public void Log(string message)
    {
        Console.WriteLine("LOG: " + message);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Logger logger1 = Logger.GetInstance();
        Logger logger2 = Logger.GetInstance();
        logger1.Log("Application Started");
        logger2.Log("User Logged In");
        if (logger1 == logger2)
        {
            Console.WriteLine("Both objects are the same instance.");
        }
        else
        {
            Console.WriteLine("Different instances created.");
        }
    }
}
