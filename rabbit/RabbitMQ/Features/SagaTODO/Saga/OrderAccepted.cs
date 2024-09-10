using MassTransit;

namespace RabbitMQ.Features.Saga.Saga;

public class OrderAccepted : CorrelatedBy<Guid>
{
	public Guid CorrelationId { get; set; }
	public DateTime Timestamp { get; set; }
}
