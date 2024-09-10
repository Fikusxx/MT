using Kafka.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Kafka.Controllers;

[ApiController]
[Route("kafka")]
public class KafkaController : ControllerBase
{
    private readonly ITopicProducer<KafkaMessage> producer;
    private static readonly Guid Id = Guid.Parse("1441ab2e-7db3-48fe-8b38-f04710823c0e");

    public KafkaController(ITopicProducer<KafkaMessage> producer)
    {
        this.producer = producer;
    }

    [HttpGet]
    [Route("send-one")]
    public async Task<IActionResult> SendOne([FromQuery] bool sameId)
    {
        await producer.Produce(new KafkaMessage(sameId ? Id : Guid.NewGuid(), "Hello World"),
            Pipe.Execute<KafkaSendContext<KafkaMessage>>(ctx =>
            {
                // For testing
                // ctx.MessageId = Guid.Parse("");
                ctx.MessageId = Guid.NewGuid();

                ctx.FaultAddress = new Uri("http://kekw");
                ctx.DestinationAddress = new Uri("http://pogt");
                ctx.Headers.Set("key", "value");
                ctx.Headers.Set("time", DateTimeOffset.UtcNow);

                // explicitly define the partition
                //ctx.Partition = 1;
            }));

        return Ok();
    }

    [HttpGet]
    [Route("send-many")]
    public async Task<IActionResult> SendMany([FromQuery] bool sameId)
    {
        var tasks = new List<Task>();

        for (var i = 0; i < 20; i++)
        {
            tasks.Add(producer.Produce(new KafkaMessage(sameId ? Id : Guid.NewGuid(), "Hello World")));
        }

        await Task.WhenAll(tasks);

        return Ok();
    }

    [HttpGet]
    [Route("complicated-1")]
    public async Task<IActionResult> One([FromServices] ITopicProducer<string, ComplicatedKafkaMessage> producer)
    {
        var options = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        var message = new MessageOne("one");
        var e = new ComplicatedKafkaMessage()
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(MessageOne),
            Event = JsonConvert.SerializeObject(message, options),
            Message = message
        };

        await producer.Produce("123", e);

        return Ok();
    }

    [HttpGet]
    [Route("complicated-2")]
    public async Task<IActionResult> Two([FromServices] ITopicProducer<string, ComplicatedKafkaMessage> producer)
    {
        var options = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        var e = new ComplicatedKafkaMessage()
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(MessageTwo),
            Event = JsonConvert.SerializeObject(new MessageTwo("two"), options)
        };

        await producer.Produce("123", e);

        return Ok();
    }
}