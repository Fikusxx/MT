using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orchestrator.Events;
using Orchestrator.StateMachine;

namespace Orchestrator;

public static class DependencyInjection
{
    public static IServiceCollection AddTransport(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(massTransit =>
        {
            massTransit.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
            massTransit.AddRider(rider =>
            {
                // rider.AddTransient<IErrorTransport, FaultTransport>();

                rider
                    .AddSagaStateMachine<CommunicationStateMachine, CommunicationState>(
                        typeof(CommunicationStateMachineDefinition))
                    .EntityFrameworkRepository(opt =>
                    {
                        // opt.ConcurrencyMode = ConcurrencyMode.Pessimistic; // uses FOR UPDATE
                        opt.ConcurrencyMode = ConcurrencyMode.Optimistic; // requires RowVersion

                        opt.AddDbContext<DbContext, CommunicationStateDbContext>((provider, builder) =>
                        {
                            builder.UseNpgsql(
                                "Host=localhost;Port=5432;Database=Sms;Username=postgres;Search Path=skk;Password=postgres",
                                m => { m.MigrationsHistoryTable("__EFMigrationsHistory", "skk"); });
                        });

                        //This line is added to enable PostgreSQL features
                        opt.UsePostgres();
                    });

                rider.AddProducer<long, CommunicationEvent>("start");
                rider.AddProducer<Guid, FinalEvent>("end");

                rider.AddProducer<Guid, SendSmsEvent>("sms");
                rider.AddProducer<Guid, SmsStatusEvent>("sms-status");
                rider.AddProducer<Guid, SendPushEvent>("push");
                rider.AddProducer<long, PushStatusEvent>("push-status");

                rider.UsingKafka(new ClientConfig { BootstrapServers = "localhost" }, (ctx, cfg) =>
                {
                    cfg.TopicEndpoint<long, CommunicationEvent>(
                        topicName: "start",
                        groupId: "start",
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureSaga<CommunicationState>(ctx);
                        });

                    // cfg.TopicEndpoint<Guid, SmsStatusEvent>(
                    //     topicName: "sms-status",
                    //     groupId: "sms-status",
                    //     configure: topicConfig =>
                    //     {
                    //         topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                    //         topicConfig.ConfigureSaga<CommunicationState>(ctx);
                    //     });
                    //
                    cfg.TopicEndpoint<long, PushStatusEvent>(
                        topicName: "push-status",
                        groupId: "push-status",
                        configure: topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureSaga<CommunicationState>(ctx);
                        });
                });
            });
        });

        return services;
    }
}