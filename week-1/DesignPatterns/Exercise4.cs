using System;

interface IPaymentProcessor
{
    void ProcessPayment(double amount);
}

class PayPalGateway
{
    public void MakePayment(double amount)
    {
        Console.WriteLine($"Payment of Rs.{amount} processed through PayPal");
    }
}

class StripeGateway
{
    public void Pay(double amount)
    {
        Console.WriteLine($"Payment of Rs.{amount} processed through Stripe");
    }
}

class PayPalAdapter : IPaymentProcessor
{
    private PayPalGateway gateway = new PayPalGateway();

    public void ProcessPayment(double amount)
    {
        gateway.MakePayment(amount);
    }
}

class StripeAdapter : IPaymentProcessor
{
    private StripeGateway gateway = new StripeGateway();

    public void ProcessPayment(double amount)
    {
        gateway.Pay(amount);
    }
}

class Program
{
    static void Main()
    {
        IPaymentProcessor paypal = new PayPalAdapter();
        paypal.ProcessPayment(1000);

        IPaymentProcessor stripe = new StripeAdapter();
        stripe.ProcessPayment(2000);
    }
}