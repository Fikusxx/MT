using MassTransit;
using Orchestrator.StateMachine.Middlewares;

namespace Orchestrator.StateMachine;

public sealed class CommunicationStateMachineDefinition : SagaDefinition<CommunicationState>
{
    public CommunicationStateMachineDefinition()
    {
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<CommunicationState> sagaConfigurator,
        IRegistrationContext context)
    {
        // Configuring a filter for all the registered events in the state machine
        var logger = context.GetRequiredService<ILogger<SagaLoggingMiddlewareFilter<CommunicationState>>>();
        sagaConfigurator.UseFilter(new SagaLoggingMiddlewareFilter<CommunicationState>(logger));

        // endpointConfigurator.ConfigureError(x =>
        // {
        //     x.UseFilters(
        //         new FaultProcessingMiddlewareFilter(context.GetRequiredService<IErrorTransport>()),
        //         new ErrorTransportFilter());
        // });

        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}