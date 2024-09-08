using MassTransit;

namespace RabbitMQ.Features.Exceptions;

public sealed record MessageConsumer : IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        if (context.Message.IsValid)
            Console.WriteLine("Valid...");

        throw new Exception("Not valid...");
    }
}

public sealed record FaultMessageConsumer : IConsumer<Fault<Message>>
{
    public Task Consume(ConsumeContext<Fault<Message>> context)
    {
        Console.WriteLine("Processed faulted message...");

        return Task.CompletedTask;
    }
}

/// <summary>
/// While you can consume messages from error queues, it isn't highly recommended.
/// It's better to use a tool to move those messages into a different queue for reprocessing (sometimes the original queue,
/// but care should be taken to avoid messages being moved too many times).
/// </summary>
public sealed record ErrorMessageConsumer : IConsumer<Message>
{
    public Task Consume(ConsumeContext<Message> context)
    {
        Console.WriteLine("Final Error Message Processing...");
        
        return Task.CompletedTask;
    }
}