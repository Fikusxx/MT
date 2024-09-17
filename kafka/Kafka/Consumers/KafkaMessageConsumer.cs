using Kafka.Messages;
using MassTransit;

namespace Kafka.Consumers;

public sealed class KafkaMessageConsumer : IConsumer<KafkaMessage>
{
    public async Task Consume(ConsumeContext<KafkaMessage> context)
    {
        await Task.Delay(2000);
        var key = context.GetKey<Guid>();
        var partition = context.Partition();
        var offset = context.Offset();
        var messageId = context.MessageId;
        var retry = context.GetRetryAttempt();
        var header = context.Headers.Get<string>("key");
        var value = context.Headers.FirstOrDefault(x => x.Key == "key").Value;
        var time2 = context.Headers.Get<DateTimeOffset>("time");

        var timeValue = time2?.ToLocalTime() ?? DateTimeOffset.UtcNow.ToLocalTime();

        Console.WriteLine($"{messageId} : key {key} partition {partition} offset {offset} at {timeValue} | Address: {this.GetAddress():X}");

        // can be piped or used as is due to high events cohesion for granularity
        // if (retry == 2)
        // {
        // 	var topicProducer = context.GetServiceOrCreateInstance<ITopicProducer<KafkaMessageError>>();
        // 	await topicProducer.Produce(new KafkaMessageError());
        // 	return;
        // }
    }
}

public sealed class KafkaMessageErrorConsumer : IConsumer<KafkaMessageError>
{
    public Task Consume(ConsumeContext<KafkaMessageError> context)
    {
        Console.WriteLine("Starting... #2");

        return Task.CompletedTask;
    }
}