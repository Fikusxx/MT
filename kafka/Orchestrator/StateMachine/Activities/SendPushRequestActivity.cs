using MassTransit;
using Orchestrator.Events;

namespace Orchestrator.StateMachine.Activities;

public sealed class SendPushRequestActivity : IStateMachineActivity<CommunicationState, CommunicationEvent>
{
    private readonly ITopicProducer<Guid, SendPushEvent> producer;
    private readonly ILogger<SendPushRequestActivity> logger;

    public SendPushRequestActivity(ITopicProducer<Guid, SendPushEvent> producer, ILogger<SendPushRequestActivity> logger)
    {
        this.producer = producer;
        this.logger = logger;
    }

    public async Task Execute(BehaviorContext<CommunicationState, CommunicationEvent> context,
        IBehavior<CommunicationState, CommunicationEvent> next)
    {
        logger.LogInformation("Producing SendPushEvent event...");
        await producer.Produce(Guid.NewGuid(), new SendPushEvent { Id = Guid.NewGuid(), Text = "Sending push..." });

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<CommunicationState, CommunicationEvent, TException> context,
        IBehavior<CommunicationState, CommunicationEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }

    void IProbeSite.Probe(ProbeContext context) => context.CreateScope(nameof(SendPushRequestActivity));
    void IVisitable.Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}