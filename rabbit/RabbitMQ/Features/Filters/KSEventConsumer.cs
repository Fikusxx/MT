using MassTransit;

namespace RabbitMQ.Features.Filters;

public sealed record KsEventConsumer : IConsumer<KsEvent>
{
    public Task Consume(ConsumeContext<KsEvent> context)
    {
        Console.WriteLine("Starting processing KsEvent...");
        throw new Exception();
    }
}