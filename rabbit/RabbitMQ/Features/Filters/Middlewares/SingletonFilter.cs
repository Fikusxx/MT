using MassTransit;
using MassTransit.Configuration;

namespace RabbitMQ.Features.Filters.Middlewares;

public static class SingletonFilterConfiguratorExtensions
{
    public static void UseSingletonFilter<T>(this IPipeConfigurator<T> configurator, IBusRegistrationContext ctx)
        where T : class, PipeContext
    {
        var service = ctx.GetRequiredService<DummySingletonService>();
        configurator.AddPipeSpecification(new SingletonFilterSpecification<T>(service));
    }
}

public class SingletonFilterSpecification<T> : IPipeSpecification<T> where T : class, PipeContext
{
    private readonly DummySingletonService _service;

    public SingletonFilterSpecification(DummySingletonService service)
    {
        _service = service;
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }

    public void Apply(IPipeBuilder<T> builder)
    {
        builder.AddFilter(new SingletonFilter<T>(_service));
    }
}

public class SingletonFilter<T> : IFilter<T> where T : class, PipeContext
{
    private readonly DummySingletonService service;

    public SingletonFilter(DummySingletonService service)
    {
        this.service = service;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("singleton filter");
    }

    public async Task Send(T context, IPipe<T> next)
    {
        await Console.Out.WriteLineAsync($"Filter: Pre | Type: {typeof(T).Name}");
        service.Call();

        await next.Send(context);

        await Console.Out.WriteLineAsync($"Filter: Post | Type: {typeof(T).Name}");
    }
}