using MassTransit;

namespace RabbitMQ.Features.Filters.Middlewares;

public sealed class ContextSendFilter<T> : IFilter<SendContext<T>> where T : class
{
	public void Probe(ProbeContext context)
	{
		context.CreateFilterScope("y do i exist");
	}

	public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
	{
		context.Headers.Set("applicationId", "");
		context.Headers.Set("tenantId", "");
		context.Headers.Set("lang", "fr");
		await next.Send(context);
	}
}