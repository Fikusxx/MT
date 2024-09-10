using System.Reflection;
using Confluent.Kafka;
using Kafka.Config;
using MassTransit;

namespace Kafka;

public static class DependencyInjection
{
    public static void AddKafka(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            // we aren't using the bus, only Kafka
            x.UsingInMemory();
            // x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

            x.AddRider(rider =>
            {
                // Consumers
                // rider.AddConsumer<KafkaMessageConsumer>();
                // rider.AddConsumer<KafkaMessageErrorConsumer>();
                // rider.AddConsumer<ComplicatedKafkaMessageConsumer>();

                // rider.AddProducer<string, KafkaMessageError>("error");
                //
                // rider.AddProducer<string, ComplicatedKafkaMessage>("demo");
                
                rider.AddMessageProducer();

                rider.UsingKafka((ctx, cfg) =>
                {
                    //cfg.UseSendFilter(typeof(ContextSendFilter<>), context);

                    cfg.ClientId = Assembly.GetExecutingAssembly().GetName().Name;

                    #region Host

                    cfg.Host("localhost:9092");

                    // cfg.SecurityProtocol = SecurityProtocol.SaslSsl;

                    // cfg.Host("localhost:9092", h =>
                    // {
                    //     h.UseSasl(s =>
                    //     {
                    //         s.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                    //         s.Mechanism = SaslMechanism.Plain;
                    //         s.Username = null;
                    //         s.Password = null;
                    //     });
                    // });

                    #endregion
                    

                    cfg.AddMessageEndpoint(ctx);
                    cfg.AddExtraEndpoint();
                });
            });
        });
    }
}