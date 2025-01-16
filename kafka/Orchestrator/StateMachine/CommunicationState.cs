using MassTransit;

namespace Orchestrator.StateMachine;

public sealed class CommunicationState : SagaStateMachineInstance
{
    public required Guid CorrelationId { get; set; }
    public required long StateId { get; set; }
    public required string CurrentState { get; set; }
    public required string Text { get; set; }
    public required uint RowVersion { get; set; }
}