using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Orchestrator.StateMachine;

public sealed class CommunicationStateDbContext : SagaDbContext
{
    public CommunicationStateDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new CommunicationStateMap(); }
    }
}