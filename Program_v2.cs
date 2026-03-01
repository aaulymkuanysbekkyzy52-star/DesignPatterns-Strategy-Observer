using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#region ===== STRATEGY PATTERN =====

public enum ServiceClass
{
    Economy,
    Business
}

public class TripRequest
{
    public double DistanceKm { get; set; }
    public int Passengers { get; set; }
    public ServiceClass Class { get; set; }
    public bool IsChildDiscount { get; set; }
    public bool IsPensionerDiscount { get; set; }
}

public interface ICostCalculationStrategy
{
    decimal CalculateCost(TripRequest request);
}

// ✈️ Airplane
public class AirplaneStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripRequest r)
    {
        decimal basePrice = (decimal)r.DistanceKm * 0.5m;
        return ApplyFactors(basePrice, r);
    }
}

// 🚆 Train
public class TrainStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripRequest r)
    {
        decimal basePrice = (decimal)r.DistanceKm * 0.3m;
        return ApplyFactors(basePrice, r);
    }
}

// 🚌 Bus
public class BusStrategy : ICostCalculationStrategy
{
    public decimal CalculateCost(TripRequest r)
    {
        decimal basePrice = (decimal)r.DistanceKm * 0.15m;
        return ApplyFactors(basePrice, r);
    }
}

static decimal ApplyFactors(decimal basePrice, TripRequest r)
{
    if (r.Class == ServiceClass.Business)
        basePrice *= 1.5m;

    if (r.IsChildDiscount)
        basePrice *= 0.7m;

    if (r.IsPensionerDiscount)
        basePrice *= 0.8m;

    return basePrice * r.Passengers;
}

public class TravelBookingContext
{
    private ICostCalculationStrategy _strategy;

    public void SetStrategy(ICostCalculationStrategy strategy)
    {
        _strategy = strategy ?? throw new Exception("Стратегия не выбрана");
    }

    public decimal Calculate(TripRequest request)
    {
        if (request.DistanceKm <= 0 || request.Passengers <= 0)
            throw new Exception("Неверные данные поездки");

        return _strategy.CalculateCost(request);
    }
}

#endregion

#region ===== OBSERVER PATTERN =====

public interface IObserver
{
    Task UpdateAsync(string stock, decimal price);
}

public interface ISubject
{
    void Subscribe(string stock, IObserver observer);
    void Unsubscribe(string stock, IObserver observer);
    Task NotifyAsync(string stock, decimal price);
}

public class StockExchange : ISubject
{
    private readonly Dictionary<string, List<IObserver>> _subscriptions = new();

    public void Subscribe(string stock, IObserver observer)
    {
        if (!_subscriptions.ContainsKey(stock))
            _subscriptions[stock] = new List<IObserver>();

        _subscriptions[stock].Add(observer);
        Console.WriteLine($"[LOG] Подписка: {observer.GetType().Name} -> {stock}");
    }

    public void Unsubscribe(string stock, IObserver observer)
    {
        _subscriptions[stock]?.Remove(observer);
        Console.WriteLine($"[LOG] Отписка: {observer.GetType().Name} -> {stock}");
    }

    public async Task ChangePriceAsync(string stock, decimal price)
    {
        Console.WriteLine($"\n[Биржа] {stock} новая цена: {price}");
        await NotifyAsync(stock, price);
    }

    public async Task NotifyAsync(string stock, decimal price)
    {
        if (!_subscriptions.ContainsKey(stock)) return;

        foreach (var obs in _subscriptions[stock])
            await obs.UpdateAsync(stock, price);
    }
}

// 👤 Trader
public class TraderObserver : IObserver
{
    public async Task UpdateAsync(string stock, decimal price)
    {
        await Task.Delay(300);
        Console.WriteLine($"[Трейдер] {stock} = {price}");
    }
}

// 🤖 Robot
public class TradingRobotObserver : IObserver
{
    private readonly decimal _limit;

    public TradingRobotObserver(decimal limit)
    {
        _limit = limit;
    }

    public async Task UpdateAsync(string stock, decimal price)
    {
        await Task.Delay(300);
        if (price < _limit)
            Console.WriteLine($"[РОБОТ] Покупка {stock} по {price}");
        else
            Console.WriteLine($"[РОБОТ] Продажа {stock} по {price}");
    }
}

#endregion

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== TRAVEL BOOKING ===");

        var context = new TravelBookingContext();
        context.SetStrategy(new AirplaneStrategy());

        var request = new TripRequest
        {
            DistanceKm = 1200,
            Passengers = 2,
            Class = ServiceClass.Business,
            IsChildDiscount = false,
            IsPensionerDiscount = true
        };

        Console.WriteLine($"Стоимость поездки: {context.Calculate(request)} ₸");

        Console.WriteLine("\n=== STOCK EXCHANGE ===");

        var exchange = new StockExchange();
        var trader = new TraderObserver();
        var robot = new TradingRobotObserver(100);

        exchange.Subscribe("AAPL", trader);
        exchange.Subscribe("AAPL", robot);

        await exchange.ChangePriceAsync("AAPL", 95);
        await exchange.ChangePriceAsync("AAPL", 120);
    }
}
