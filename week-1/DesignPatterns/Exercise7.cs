using System;
using System.Collections.Generic;

interface IObserver
{
    void Update(double price);
}

interface IStock
{
    void Register(IObserver observer);
    void Deregister(IObserver observer);
    void NotifyObservers();
}

class StockMarket : IStock
{
    private List<IObserver> observers = new List<IObserver>();
    private double stockPrice;

    public void SetPrice(double price)
    {
        stockPrice = price;
        NotifyObservers();
    }

    public void Register(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Deregister(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.Update(stockPrice);
        }
    }
}

class MobileApp : IObserver
{
    public void Update(double price)
    {
        Console.WriteLine($"Mobile App: Stock Price = {price}");
    }
}

class WebApp : IObserver
{
    public void Update(double price)
    {
        Console.WriteLine($"Web App: Stock Price = {price}");
    }
}

class Program
{
    static void Main()
    {
        StockMarket stock = new StockMarket();

        stock.Register(new MobileApp());
        stock.Register(new WebApp());

        stock.SetPrice(1500);
        stock.SetPrice(1700);
    }
}