using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Features.Filters;

public static class FilterEndpoints
{
    private const string Name = "Filter";

    public static IEndpointRouteBuilder MapFilterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(Name, async (
                [FromQuery] bool shouldFilter,
                [FromServices] IPublishEndpoint broker) =>
            {
                await broker.Publish(new FilteredEvent { ShouldFilter = shouldFilter });

                return Results.Ok();
            })
            .WithName(Name);

        return app;
    }
}