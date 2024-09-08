using RabbitMQ.Client;

namespace RabbitMQ.Features.Native;

public class RabbitConnection
{
	private readonly IConnection connection;

    public RabbitConnection()
    {
		var factory = new ConnectionFactory();
		factory.Uri = new Uri("123");

		connection = factory.CreateConnection();
	}

	public IModel CreateChannel()
	{
		return connection.CreateModel();
	}
}
