using MassTransit;

namespace RabbitMQ.Features.Filters.Middlewares;

public sealed record AllTypesConsumeScopeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        Console.WriteLine($"{GetType().Name} executed...");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("all types consume scoped filter");
    }
}

public sealed record FilteredEventConsumeScopeFilter : IFilter<ConsumeContext<FilteredEvent>>
{
    private readonly DummyScopedService _scopedService;

    public FilteredEventConsumeScopeFilter(DummyScopedService _scopedService)
    {
        this._scopedService = _scopedService;
    }

    public async Task Send(ConsumeContext<FilteredEvent> context, IPipe<ConsumeContext<FilteredEvent>> next)
    {
        Console.WriteLine($"{GetType().Name} executed...");

        if (_scopedService.GetChecked())
            return;
        
        _scopedService.SetChecked();
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("filtered event consume scoped filter");
    }
}

public sealed record AllTypesPublishScopeFilter<T> : IFilter<PublishContext<T>> where T : class
{
    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        Console.WriteLine($"{GetType().Name} executed...");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("all types publish scope filter");
    }
}

public sealed record FilteredEventPublishScopeFilter : IFilter<PublishContext<FilteredEvent>>
{
    public async Task Send(PublishContext<FilteredEvent> context, IPipe<PublishContext<FilteredEvent>> next)
    {
        Console.WriteLine($"{GetType().Name} executed...");
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("specific type publish scope filter");
    }
}