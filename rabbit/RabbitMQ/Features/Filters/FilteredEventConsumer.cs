using MassTransit;

namespace RabbitMQ.Features.Filters;

public sealed class FilteredEventConsumer : IConsumer<FilteredEvent>
{
    public Task Consume(ConsumeContext<FilteredEvent> context)
    {
        Console.WriteLine("Filtered consumer executed...");

        return Task.CompletedTask;
    }
}