using MassTransit;

namespace Orchestrator.StateMachine.Middlewares;

/// <summary>
/// Any generic logging goes here
/// </summary>
public sealed class SagaLoggingMiddlewareFilter<TSaga> : IFilter<SagaConsumeContext<TSaga>>
    where TSaga : class, ISaga
{
    private readonly ILogger<SagaLoggingMiddlewareFilter<TSaga>> logger;

    public SagaLoggingMiddlewareFilter(ILogger<SagaLoggingMiddlewareFilter<TSaga>> logger)
    {
        this.logger = logger;
    }

    async Task IFilter<SagaConsumeContext<TSaga>>.Send(
        SagaConsumeContext<TSaga> context,
        IPipe<SagaConsumeContext<TSaga>> next)
    {
        var hasMessage = context.TryGetMessage<object>(out var consumeContext);
        
        if (hasMessage && consumeContext is not null)
        {
            logger.LogInformation($"Consuming {consumeContext.Message.GetType().Name}");
        }

        await next.Send(context);
    }

    void IProbeSite.Probe(ProbeContext context)
        => context.CreateScope(nameof(SagaLoggingMiddlewareFilter<TSaga>));
}