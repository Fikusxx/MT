using MassTransit;
using RabbitMQ.Features.Direct.Events;

namespace RabbitMQ.Features.Direct.Consumers;

public sealed record SlowOrdersConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine("Very slow.. yikes, not gonna order again :(");
        return Task.CompletedTask;
    }
}