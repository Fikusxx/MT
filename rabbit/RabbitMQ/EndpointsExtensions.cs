using RabbitMQ.Features.Batch;
using RabbitMQ.Features.Direct.Endpoints;
using RabbitMQ.Features.Exceptions;

namespace RabbitMQ;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDirectEndpoints();
        app.MapBatchEndpoints();
        app.MapExceptionEndpoints();

        return app;
    }
}