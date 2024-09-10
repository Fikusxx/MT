namespace Kafka.Messages;

public class ComplicatedKafkaMessage
{
	public Guid EventId { get; set; }
	public string EventType { get; set; }

	// other meta data

	public string Event { get; set; }
	public IMessage Message { get; set; }	
}

public record MessageOne(string Value) : IMessage
{ }
public record MessageTwo(string Value) : IMessage
{ }

public interface IMessage;