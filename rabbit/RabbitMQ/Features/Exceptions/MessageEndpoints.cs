using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Features.Exceptions;

public static class MessageEndpoints
{
    private const string Name = "Exception";

    public static IEndpointRouteBuilder MapExceptionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(Name, async (
                [FromQuery] bool valid,
                [FromServices] IPublishEndpoint broker) =>
            {
                await broker.Publish(new Message { IsValid = valid });

                return Results.Ok();
            })
            .WithName(Name);

        return app;
    }
}