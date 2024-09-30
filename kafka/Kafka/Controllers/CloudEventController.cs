using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Kafka.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Controllers;

[ApiController]
[Route("cloud")]
public class CloudEventController : ControllerBase
{
    private readonly ITopicProducerProvider topicProducerProvider;

    public CloudEventController(ITopicProducerProvider topicProducerProvider)
    {
        this.topicProducerProvider = topicProducerProvider;
    }

    [HttpPost("first")]
    public async Task<IActionResult> ProduceToFirstTopic()
    {
        var producer = topicProducerProvider.GetProducer<Guid, CloudEvent>(new Uri($"topic:{Constants.FirstCouldEventTopic}"));
        
        var e = new CloudEvent
        {
            Id = "first-event-id",
            Type = "first-event-type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.UtcNow,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new FirstCloudEventData()
        };
        e.SetAttributeFromString("correlationid", Guid.NewGuid().ToString());
        
        await producer.Produce(Guid.NewGuid(), e, Pipe.Execute<KafkaSendContext<Guid, CloudEvent>>(context =>
        {
            context.ValueSerializer = new CloudEventSerializer();
        }));

        return Ok();
    }
    
    [HttpPost("second")]
    public async Task<IActionResult> ProduceToSecondTopic()
    {
        var producer = topicProducerProvider.GetProducer<Guid, CloudEvent>(new Uri($"topic:{Constants.SecondCouldEventTopic}"));
        
        var e = new CloudEvent
        {
            Id = "second-event-id",
            Type = "second-event-type",
            Source = new Uri("https://cloudevents.io/"),
            Time = DateTimeOffset.UtcNow,
            DataContentType = MediaTypeNames.Application.Json,
            Data = new FirstCloudEventData()
        };
        e.SetAttributeFromString("correlationid", Guid.NewGuid().ToString());
        
        await producer.Produce(Guid.NewGuid(), e, Pipe.Execute<KafkaSendContext<Guid, CloudEvent>>(context =>
        {
            context.ValueSerializer = new CloudEventSerializer();
        }));
        
        return Ok();
    }
}

class CloudEventSerializer: IAsyncSerializer<CloudEvent>
{
    public Task<byte[]> SerializeAsync(CloudEvent data, SerializationContext context)
    {
        var formatter = new JsonEventFormatter();
        var element = formatter.ConvertToJsonElement(data);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}