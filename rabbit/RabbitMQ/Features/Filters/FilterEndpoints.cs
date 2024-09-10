using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Features.Filters;

public static class FilterEndpoints
{
    private const string Name = "Filter";
    private const string Ks = "Ks";

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

        app.MapGet(Ks, async (
                [FromServices] IPublishEndpoint broker) =>
            {
                var tasks = new List<Task>();

                for (var i = 0; i < 20; i++)
                {
                    tasks.Add(broker.Publish(new KsEvent()));
                }

                await Task.WhenAll(tasks);

                return Results.Ok();
            })
            .WithName(Ks);

        return app;
    }
}