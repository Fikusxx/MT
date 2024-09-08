namespace RabbitMQ.Features.Exceptions;

public sealed record Message
{
    public required bool IsValid { get; init; }
}