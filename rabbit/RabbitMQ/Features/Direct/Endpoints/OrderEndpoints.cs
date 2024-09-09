using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Features.Direct.Events;

namespace RabbitMQ.Features.Direct.Endpoints;

public static class OrderEndpoints
{
    private const string Name = "Direct";
    private const string Batch = "Direct/batch";

    public static IEndpointRouteBuilder MapDirectEndpoints(this IEndpointRouteBuilder app)
    {
        // priority should be either fast/slow
        app.MapGet(Name, async (
                [FromQuery] string priority,
                [FromServices] IPublishEndpoint broker) =>
            {
                await broker.Publish(new OrderCreatedEvent { Number = "#1", Priority = priority });

                return Results.Ok();
            })
            .WithName(Name);

        app.MapGet(Batch, async (
                [FromQuery] string priority,
                [FromServices] IPublishEndpoint broker) =>
            {
                var tasks = new List<Task>();

                for (var i = 0; i < 100; i++)
                {
                    tasks.Add(broker.Publish(new OrderCreatedEvent { Number = "#1", Priority = priority }));
                }

                await Task.WhenAll(tasks);

                return Results.Ok();
            })
            .WithName(Batch);


        return app;
    }
}