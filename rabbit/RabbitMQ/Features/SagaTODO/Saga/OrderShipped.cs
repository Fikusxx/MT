namespace RabbitMQ.Features.Saga.Saga;

public record OrderShipped
{
	public Guid OrderId { get; set; }
	public DateTime ShipDate { get; set; }
}
