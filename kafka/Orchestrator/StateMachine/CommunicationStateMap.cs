using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orchestrator.StateMachine;

public class CommunicationStateMap : SagaClassMap<CommunicationState>
{
    protected override void Configure(EntityTypeBuilder<CommunicationState> entity, ModelBuilder model)
    {
        entity.HasIndex(x => x.StateId);
        // entity.Property(x => x.StateId).ValueGeneratedNever();
        // entity.Ignore(x => x.CorrelationId);
        
        
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        
        // entity.Property(x => x.OrderDate);
        
        entity.Property(x => x.RowVersion)
            .IsRowVersion();

        // entity.Property(x => x.RowVersion)
        //     .HasColumnName("xmin")
        //     .HasColumnType("xid")
        //     .IsRowVersion();
    }
}