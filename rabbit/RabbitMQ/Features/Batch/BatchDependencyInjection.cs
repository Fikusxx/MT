using MassTransit;

namespace RabbitMQ.Features.Batch;

public static class BatchDependencyInjection
{
    public static void ConfigureBatch(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("batch", x =>
        {
            x.Consumer<BatchEventConsumer>();
            
            // number of messages pushed by the broker to internal memory buffer of logical consumer
            x.PrefetchCount = 100;
            
            // size of batch thrown at the consumer
            x.ConcurrentMessageLimit = 5;
        });
    }
}