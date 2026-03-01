using System;
using System.Collections.Generic;

#region STRATEGY PATTERN

public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата {amount} ₸ банковской картой.");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата {amount} ₸ через PayPal.");
    }
}

public class CryptoPayment : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата {amount} ₸ криптовалютой.");
    }
}

public class PaymentContext
{
    private IPaymentStrategy _strategy;

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ExecutePayment(decimal amount)
    {
        _strategy.Pay(amount);
    }
}

#endregion

#region OBSERVER PATTERN

public interface IObserver
{
    void Update(string currency, decimal rate);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

public class CurrencyExchange : ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    private string currency;
    private decimal rate;

    public void SetRate(string currency, decimal rate)
    {
        this.currency = currency;
        this.rate = rate;
        Notify();
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in observers)
        {
            observer.Update(currency, rate);
        }
    }
}

public class BankObserver : IObserver
{
    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"[Банк] {currency}: {rate}");
    }
}

public class InvestorObserver : IObserver
{
    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"[Инвестор] Анализ курса {currency}: {rate}");
    }
}

public class MobileAppObserver : IObserver
{
    public void Update(string currency, decimal rate)
    {
        Console.WriteLine($"[Мобильное приложение] {currency} = {rate}");
    }
}

#endregion

class Program
{
    static void Main()
    {
        // Strategy
        PaymentContext payment = new PaymentContext();
        payment.SetStrategy(new CreditCardPayment());
        payment.ExecutePayment(2000);

        // Observer
        CurrencyExchange exchange = new CurrencyExchange();
        exchange.Attach(new BankObserver());
        exchange.Attach(new InvestorObserver());
        exchange.Attach(new MobileAppObserver());

        exchange.SetRate("USD", 91.5m);
    }
}
