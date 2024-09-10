using MassTransit;
using RabbitMQ.Features.Filters.Middlewares;

namespace RabbitMQ.Features.Filters;

public static class FiltersDependencyInjection
{
    public static void ConfigureFilter(this IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext ctx)
    {
        // filters for all endpoints
        // generic filters inside a specific endpoints registration is a no go, just an example
        // cfg.UseConsumeFilter(typeof(AllTypesConsumeScopeFilter<>), ctx);
        // cfg.UsePublishFilter(typeof(AllTypesPublishScopeFilter<>), ctx);

        cfg.ReceiveEndpoint("filter", x =>
        {
            x.Consumer<FilteredEventConsumer>();

            // skipped messages will go to _skipped queue by default
            x.PublishFaults = false;
            x.DiscardSkippedMessages();

            // specific filter for this endpoint
            x.UseConsumeFilter<FilteredEventConsumeScopeFilter>(ctx);

            // with this scope - would only be called inside IConsumer<T>, useless 
            // x.UsePublishFilter<FilteredEventPublishScopeFilter>(ctx);
        });

        cfg.ReceiveEndpoint("ks", x =>
        {
            x.Consumer<KsEventConsumer>();
            x.PublishFaults = false;
            x.DiscardSkippedMessages();
            x.DiscardFaultedMessages();

            x.UseKillSwitch(options => options
                .SetActivationThreshold(10) // start monitoring after 10 processed message
                .SetTripThreshold(0.5) // when ex rate reaches 50% - ks activates, i.e line cb opens
                .SetRestartTimeout(TimeSpan.FromSeconds(10)) // disables message processing for 10 seconds when ks has been activated
                .SetExceptionFilter(filter => filter.Handle<Exception>()) // handle all ex basically
                );

            x.ConcurrentMessageLimit = 1;
        });
    }
}