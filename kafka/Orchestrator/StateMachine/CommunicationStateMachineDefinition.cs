using MassTransit;

namespace Orchestrator.StateMachine;

public sealed class CommunicationStateMachineDefinition : SagaDefinition<CommunicationState>
{
    public CommunicationStateMachineDefinition()
    {
        ConcurrentMessageLimit = 5;
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<CommunicationState> sagaConfigurator,
        IRegistrationContext context)
    {
        // Retrying unhandled exceptions by the saga state machine
        sagaConfigurator.UseMessageRetry(config =>
        {
            config.Interval(3, 1000);
            // config.Ignore<NotATransientException>();
        });

        // Configuring a filter for all the registered events in the state machine
        // sagaConfigurator.UseFilter(new SagaLoggingMiddlewareFilter<OrderRequestSagaInstance>());

        // endpointConfigurator.ConfigureError(x =>
        // {
        //     x.UseFilters(
        //         new FaultProcessingMiddlewareFilter(context.GetRequiredService<IErrorTransport>()),
        //         new ErrorTransportFilter());
        // });

        base.ConfigureSaga(endpointConfigurator, sagaConfigurator, context);
    }
}