namespace RabbitMQ.Features.Saga.Models;

public class CreateSomethingCommand
{
	public string Name { get; set; } = nameof(CreateSomethingCommand);
}
