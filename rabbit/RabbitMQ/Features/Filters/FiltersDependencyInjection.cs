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
            
            // specific filter for this endpoint
            x.UseConsumeFilter<FilteredEventConsumeScopeFilter>(ctx);
            
            // with this scope - would only be called inside IConsumer<T>, useless 
            // x.UsePublishFilter<FilteredEventPublishScopeFilter>(ctx);
        });
    }
}