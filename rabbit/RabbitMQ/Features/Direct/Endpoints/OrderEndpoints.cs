using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Features.Direct.Events;

namespace RabbitMQ.Features.Direct.Endpoints;

public static class OrderEndpoints
{
    private const string Name = "Direct";

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

        return app;
    }
}