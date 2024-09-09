using MassTransit;
using MassTransit.Configuration;

namespace RabbitMQ.Features.Filters.Middlewares;

public static class FilterConfigurationExtensions
{
    public static void UseMessageFilter(this IConsumePipeConfigurator configurator, IBusRegistrationContext ctx)
    {
        var service = ctx.GetRequiredService<DummySingletonService>();
        var observer = new MessageFilterConfigurationObserver(configurator, service);
    }
}

public sealed class IdempotencyFilter : IFilter<ConsumeContext<FilteredEvent>>
{
    private readonly DummySingletonService service;

    public IdempotencyFilter(DummySingletonService service)
    {
        this.service = service;
    }

    public async Task Send(ConsumeContext<FilteredEvent> context, IPipe<ConsumeContext<FilteredEvent>> next)
    {
        service.Call();

        var time = context.Headers.Get<DateTimeOffset>("Time");

        Console.WriteLine($"{context.MessageId} Idempotency check start at {time?.ToLocalTime() ?? DateTime.UtcNow}");

        // imagine real check
        if (false)
            return;

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
    }
}

public class MessageFilterPipeSpecification : IPipeSpecification<ConsumeContext<FilteredEvent>>
{
    private readonly DummySingletonService service;

    public MessageFilterPipeSpecification(DummySingletonService service)
    {
        this.service = service;
    }

    public void Apply(IPipeBuilder<ConsumeContext<FilteredEvent>> builder)
    {
        var filter = new IdempotencyFilter(service);

        builder.AddFilter(filter);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}

public sealed class MessageFilterConfigurationObserver : ConfigurationObserver, IMessageConfigurationObserver
{
    private readonly DummySingletonService _service;

    public MessageFilterConfigurationObserver(IConsumePipeConfigurator receiveEndpointConfigurator,
        DummySingletonService service)
        : base(receiveEndpointConfigurator)
    {
        _service = service;
        Connect(this);
    }

    public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator) where TMessage : class
    {
        var specification = new MessageFilterPipeSpecification(_service);

        configurator.AddPipeSpecification(specification);
    }
}