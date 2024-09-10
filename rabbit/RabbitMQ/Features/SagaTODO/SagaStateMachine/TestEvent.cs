namespace RabbitMQ.Features.Saga.SagaStateMachine;

public class TestEvent
{
	public Guid OrderId { get; set; }	
	public string CurrentState { get; set; }
}