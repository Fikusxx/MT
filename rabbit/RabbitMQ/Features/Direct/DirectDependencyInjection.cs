using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Features.Direct.Consumers;
using RabbitMQ.Features.Direct.Events;

namespace RabbitMQ.Features.Direct;

public static class DirectDependencyInjection
{
    public static void ConfigureDirect(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ConfigureOrderConsumers();
        cfg.ConfigureOrderMessages();
    }

    private static void ConfigureOrderConsumers(this IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint("fast-orders", x =>
        {
            x.ConfigureConsumeTopology = false;

            x.Consumer<FastOrdersConsumer>();

            x.Bind<OrderCreatedEvent>(s =>
            {
                s.RoutingKey = "fast";
                s.ExchangeType = ExchangeType.Direct;
            });
            
            // x.Bind("some-order-created-exchange", s => 
            // {
            //     s.RoutingKey = "fast";
            //     s.ExchangeType = ExchangeType.Direct;
            // });

            x.ConcurrentMessageLimit = 5;
        });

        cfg.ReceiveEndpoint("slow-orders", x =>
        {
            x.ConfigureConsumeTopology = false;

            x.Consumer<SlowOrdersConsumer>();

            x.Bind<OrderCreatedEvent>(s =>
            {
                s.RoutingKey = "slow";
                s.ExchangeType = ExchangeType.Direct;
            });
            
            // x.Bind("some-order-created-exchange", s => 
            // {
            //     s.RoutingKey = "slow";
            //     s.ExchangeType = ExchangeType.Direct;
            // });
        });
    }
    
    private static void ConfigureOrderMessages(this IRabbitMqBusFactoryConfigurator cfg)
        // private static void ConfigureOrderMessages(this IRabbitMqMessageSendTopologyConfigurator<OrderCreatedEvent> cfg)
    {
        // excluded from topology, i.e no creation for exchanges of sub-types
        cfg.Publish<IEvent>(opt =>
        {
            opt.Exclude = true;
        });
        
        // MT will create this as direct exchange
        cfg.Publish<OrderCreatedEvent>(opt =>
        {
            opt.ExchangeType = ExchangeType.Direct;
        });
        
        // override exchange name for this message when published
        cfg.Message<OrderCreatedEvent>(_ =>
        {
            // opt.SetEntityName("custom-order-created-event");
        });
        
        // use either this Send() or GlobalTopology approach
        cfg.Send<OrderCreatedEvent>(opt =>
        {
            opt.UseCorrelationId(x => x.Id);
            opt.UseRoutingKeyFormatter(x => x.Message.Priority);
        });
        
        GlobalTopology.Send.UseCorrelationId<OrderCreatedEvent>(x => x.Id);
        GlobalTopology.Send.UseRoutingKeyFormatter<OrderCreatedEvent>(x => x.Message.Priority);
    }
}