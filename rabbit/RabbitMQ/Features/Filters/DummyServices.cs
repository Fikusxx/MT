namespace RabbitMQ.Features.Filters;

public sealed record DummyScopedService
{
    private static bool HasBeenChecked;

    public void SetChecked() => HasBeenChecked = true;
    public bool GetChecked() => HasBeenChecked;
}

public sealed record DummySingletonService
{
    public void Call() => Console.WriteLine($"{GetType().Name} called");
}