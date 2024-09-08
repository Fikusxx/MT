using MassTransit;
using RabbitMQ.Features.Batch;
using RabbitMQ.Features.Direct;
using RabbitMQ.Features.Exceptions;

namespace RabbitMQ;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddTransport(this WebApplicationBuilder builder)
    {
        #region MT Host Options

        builder.Services.AddOptions<MassTransitHostOptions>()
            .Configure(opt =>
            {
                opt.WaitUntilStarted = true;
                opt.StartTimeout = null;
                opt.StopTimeout = null;
                opt.ConsumerStopTimeout = null;
            });

        #endregion

        #region MT Broker Host Options

        // hard coded values
        builder.Services.AddOptions<RabbitMqTransportOptions>()
            .Configure(opt =>
            {
                opt.Host = "localhost";
                opt.User = "guest";
                opt.Pass = "guest";
                opt.Port = 5672;
                opt.VHost = "/";
                // other options
            });

        // or just use 
        // var connection = builder.Configuration.GetConnectionString("RabbitMq")!;
        // var uri = new Uri(connection);
        // rabbit.Host(uri);

        #endregion

        builder.Services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();

            cfg.UsingRabbitMq((ctx, rabbit) =>
            {
                rabbit.ConfigureDirect();
                rabbit.ConfigureBatch();
                rabbit.ConfigureExceptions(ctx);

                rabbit.ConfigureEndpoints(ctx);
            });
        });

        return builder;
    }
}