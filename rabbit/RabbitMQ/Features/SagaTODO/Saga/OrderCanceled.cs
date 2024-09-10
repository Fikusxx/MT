using MassTransit;

namespace RabbitMQ.Features.Saga.Saga;

public class OrderCanceled : CorrelatedBy<Guid>
{
	public Guid CorrelationId { get; set; }
}
