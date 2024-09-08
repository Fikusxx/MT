using MassTransit;

namespace RabbitMQ.Features.Exceptions;

public static class ExceptionsDependencyInjection
{
    public static void ConfigureExceptions(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext ctx)
    {
        cfg.ReceiveEndpoint("exception", x =>
        {
            x.Consumer<MessageConsumer>();
            x.ConfigureConsumer<MessageConsumer>(ctx, o => o.UseInMemoryOutbox(ctx));

            // fault notifications are turned off, _error queue still works
            x.PublishFaults = false;
            // x.Consumer<FaultMessageConsumer>();

            x.UseMessageRetry(r => r.Immediate(5));
            x.UseInMemoryOutbox(ctx);
        });

        // "exception_error" is predefined by MT on errors on "exception" endpoint
        cfg.ReceiveEndpoint("exception_error", x =>
        {
            // topology is turned off to avoid self binding
            x.ConfigureConsumeTopology = false;
            x.Consumer<ErrorMessageConsumer>();
        });
    }
}