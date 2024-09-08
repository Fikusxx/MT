using MassTransit;
using RabbitMQ.Features.Direct.Events;

namespace RabbitMQ.Features.Direct.Consumers;

public sealed record FastOrdersConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine("SUPER FAST ORDER PROCESSING");
        return Task.CompletedTask;
    }
}