using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Features.Batch;

public static class BatchEndpoints
{
    private const string Name = "Batch";

    public static IEndpointRouteBuilder MapBatchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(Name, async ([FromServices] IPublishEndpoint broker) =>
            {
                var tasks = new List<Task>(10);

                for (var i = 1; i < 101; i++)
                {
                    tasks.Add(broker.Publish(new BatchEvent { Number = i.ToString() }));
                }
                
                await Task.WhenAll(tasks);

                return Results.Ok();
            })
            .WithName(Name);

        return app;
    }
}