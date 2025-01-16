using MassTransit;
using Orchestrator.Events;

namespace Orchestrator.StateMachine;

public sealed class CommunicationStateMachine : MassTransitStateMachine<CommunicationState>
{
    public CommunicationStateMachine()
    {
        RegisterStates();
        CorrelateEvents();

        Initially(
            When(CommunicationEvent)
                .Then(ctx =>
                {
                    ctx.Saga.Text = ctx.Message.Text;
                    ctx.Saga.StateId = ctx.Message.Id;
                })
                // .InitializeSaga()
                // .Activity(config => config.OfType<object>())
                .TransitionTo(CommunicationStarted));

        During(CommunicationStarted,
            When(PushStatusEvent)
                .TransitionTo(PushStatusReceived)
        );

        WhenEnter(PushStatusReceived,
            activityCallback => activityCallback
                .Finalize());

        // WhenEnter(CommunicationStarted,
        //     activityCallback => activityCallback
        //         .Finalize());
        //
        During(Final,
            Ignore(CommunicationEvent),
            Ignore(PushStatusEvent));

        // SetCompletedWhenFinalized();


        // Initially(
        //     When(CommunicationEvent)
        //         .InitializeSaga()
        //         .Activity(config => config.OfType<ReceiveOrderRequestActivity>())
        //         .TransitionTo(CommunicationStarted),
        //     When(FaultEvent)
        //         .Activity(config => config.OfType<ProcessFaultActivity>())
        //         .TransitionTo(Faulted));
        //
        // During(CommunicationStarted,
        //     When(CustomerValidationResponseEvent)
        //         .UpdateSaga()
        //         .Activity(config => config.OfType<CustomerValidationActivity>())
        //         .TransitionTo(CalculatingTaxes),
        //     When(FaultEvent)
        //         .Activity(config => config.OfType<ProcessFaultActivity>())
        //         .TransitionTo(Faulted));
        //
        // During(CalculatingTaxes,
        //     When(TaxesCalculationResponseEvent)
        //         .UpdateSaga()
        //         .Activity(config => config.OfType<TaxesCalculationActivity>())
        //         .TransitionTo(NotifyingSourceSystem),
        //     When(FaultEvent)
        //         .Activity(config => config.OfType<ProcessFaultActivity>())
        //         .TransitionTo(Faulted));
        //
        // WhenEnter(Faulted,
        //     context => context.TransitionTo(NotifyingSourceSystem));
        //
        // WhenEnter(NotifyingSourceSystem,
        //     activityCallback => activityCallback
        //         .NotifySourceSystem()
        //         .Finalize());
        //
        // During(Final,
        //     Ignore(CommunicationEvent),
        //     Ignore(CustomerValidationResponseEvent),
        //     Ignore(TaxesCalculationResponseEvent),
        //     Ignore(FaultEvent));
        //
        // // Delete finished saga instances from the repository
        // SetCompletedWhenFinalized();
    }

    private void CorrelateEvents()
    {
        Event(() => CommunicationEvent, x => x
            // .CorrelateById(m => m.Message.Id)
            .CorrelateById(m => m.StateId,  m => m.Message.Id)
            .SelectId(m => NewId.NextGuid())
            // .SelectId(m => m.Message.Id)
            .OnMissingInstance(m => m.Discard()));

        Event(() => PushStatusEvent, x => x
            .CorrelateById(m => m.StateId,  m => m.Message.Id)
            // .CorrelateById(m => m.Message.Id)
            // .SelectId(m => m.Message.Id)
            .OnMissingInstance(m => m.Fault()));
        
        
        // Event(() => TaxesCalculationResponseEvent, x => x
        //     .CorrelateById(m => m.CorrelationId ?? new Guid())
        //     .SelectId(m => m.CorrelationId ?? new Guid())
        //     .OnMissingInstance(m => m.Discard()));
        //
        // Event(() => FaultEvent, x => x
        //     .CorrelateById(m => m.CorrelationId ?? new Guid())
        //     .SelectId(m => m.CorrelationId ?? new Guid())
        //     .OnMissingInstance(m => m.Discard()));
    }

    private void RegisterStates()
        => InstanceState(x => x.CurrentState);

    public State? CommunicationStarted { get; set; }
    public State? PushStatusReceived { get; set; }

    // public State? CalculatingTaxes { get; set; }
    // public State? NotifyingSourceSystem { get; set; }
    // public State? Faulted { get; set; }
    public Event<CommunicationEvent>? CommunicationEvent { get; set; }

    public Event<PushStatusEvent>? PushStatusEvent { get; set; }
    // public Event<CustomerValidationResponseEvent>? CustomerValidationResponseEvent { get; set; }
    // public Event<TaxesCalculationResponseEvent>? TaxesCalculationResponseEvent { get; set; }
    // public Event<FaultMessageEvent>? FaultEvent { get; set; }
}