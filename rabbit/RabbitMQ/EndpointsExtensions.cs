using RabbitMQ.Features.Batch;
using RabbitMQ.Features.Direct.Endpoints;
using RabbitMQ.Features.Exceptions;
using RabbitMQ.Features.Filters;

namespace RabbitMQ;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup(string.Empty).WithTags("direct").MapDirectEndpoints();
        app.MapGroup(string.Empty).WithTags("batch").MapBatchEndpoints();
        app.MapGroup(string.Empty).WithTags("exceptions").MapExceptionEndpoints();
        app.MapGroup(string.Empty).WithTags("filter").MapFilterEndpoints();

        return app;
    }
}

/// <summary>
/// https://www.iditect.com/faq/csharp/get-memory-address-of-net-object-c.html?ysclid=m0ukaxqwo814669434
/// </summary>
public static unsafe class ObjectExtensions
{
    public static IntPtr GetAddress(this object obj)
    {
        TypedReference tr = __makeref(obj);
        IntPtr ptr = **(IntPtr**)(&tr);
        return ptr;
    }
}