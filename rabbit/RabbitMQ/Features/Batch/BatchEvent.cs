using MassTransit;

namespace RabbitMQ.Features.Batch;

public sealed record BatchEvent : IConsumer
{
    public required string Number { get; init; }
}