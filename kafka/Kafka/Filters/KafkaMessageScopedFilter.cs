using Kafka.Messages;
using MassTransit;

namespace Kafka.Filters;

public class KafkaMessageScopedFilter : IFilter<ConsumeContext<KafkaMessage>>
{
    public async Task Send(ConsumeContext<KafkaMessage> context, IPipe<ConsumeContext<KafkaMessage>> next)
    {
        Console.WriteLine($"{GetType().Name} executed...");

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("filtered event consume scoped filter");
    }
}