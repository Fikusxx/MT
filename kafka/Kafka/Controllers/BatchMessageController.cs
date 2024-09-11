using Kafka.Messages;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Controllers;

[ApiController]
[Route("batch")]
public sealed class BatchMessageController : ControllerBase
{
    private readonly ITopicProducer<BatchKafkaMessage> producer;
    private static readonly Guid Id = Guid.Parse("1441ab2e-7db3-48fe-8b38-f04710823c0e");

    public BatchMessageController(ITopicProducer<BatchKafkaMessage> producer)
    {
        this.producer = producer;
    }

    /// <summary>
    /// sameId simulates ConcurrentMessageLimit & ConcurrentDeliveryLimit logic
    /// </summary>
    [HttpGet]
    [Route("send-batch")]
    public async Task<IActionResult> SendBatch([FromQuery] bool sameId)
    {
        var tasks = new List<Task>();

        for (var i = 0; i < 20; i++)
        {
            tasks.Add(producer.Produce(new BatchKafkaMessage(sameId ? Id : Guid.NewGuid(), "Hello World")));
        }

        await Task.WhenAll(tasks);

        return Ok();
    }
}