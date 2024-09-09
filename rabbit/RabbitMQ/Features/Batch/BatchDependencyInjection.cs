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
            // https://stackoverflow.com/questions/70439261/how-does-concurrency-limit-work-in-masstransit-rabbitmq
            // https://www.youtube.com/watch?v=M_yPAhWgvo4
            x.ConcurrentMessageLimit = 5;
        });
    }
}