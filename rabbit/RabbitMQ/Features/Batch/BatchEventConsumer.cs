using MassTransit;

namespace RabbitMQ.Features.Batch;

public sealed record BatchEventConsumer : IConsumer<Batch<BatchEvent>>
{
    public Task Consume(ConsumeContext<Batch<BatchEvent>> context)
    {
        foreach (var t in context.Message)
        {
            var message = t.Message;
            Console.WriteLine($"Number: {message.Number}");
        }

        Thread.Sleep(1000);

        return Task.CompletedTask;
    }
}

public sealed class BatchEventConsumerDefinition : ConsumerDefinition<BatchEventConsumer>
{
    public BatchEventConsumerDefinition()
    {
        // size of batch thrown at the consumer
        ConcurrentMessageLimit = 7;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<BatchEventConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // number of messages pushed by the broker to internal memory buffer of logical consumer
        endpointConfigurator.PrefetchCount = 100;

        // size of batch thrown at the ALL consumers of this endpoint
        endpointConfigurator.ConcurrentMessageLimit = 10;
        
        // size of batch thrown at this specific consumers type
        consumerConfigurator.ConcurrentMessageLimit = 10;
    }
}