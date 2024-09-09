using MassTransit;
using RabbitMQ.Features.Batch;
using RabbitMQ.Features.Direct;
using RabbitMQ.Features.Exceptions;
using RabbitMQ.Features.Filters;
using RabbitMQ.Features.Filters.Middlewares;

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
        // builder.Services.AddOptions<RabbitMqTransportOptions>()
        //     .Configure(opt =>
        //     {
        //         opt.Host = "localhost";
        //         opt.User = "guest";
        //         opt.Pass = "guest";
        //         opt.Port = 5672;
        //         opt.VHost = "/";
        //         // other options
        //     });

        // or just use 
        // var connection = builder.Configuration.GetConnectionString("RabbitMq")!;
        // var uri = new Uri(connection);
        // rabbit.Host(uri, h => { });

        #endregion

        builder.Services.AddScoped<DummyScopedService>();
        builder.Services.AddSingleton<DummySingletonService>();

        builder.Services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();

            cfg.UsingRabbitMq((ctx, rabbit) =>
            {
                // rabbit.UseSingletonFilter(ctx);
                // rabbit.UseMessageFilter(ctx);
                
                var connection = builder.Configuration.GetConnectionString("RabbitMq")!;
                rabbit.ConfigureHost(connection);
                
                rabbit.ConfigureDirect();
                rabbit.ConfigureBatch();
                rabbit.ConfigureExceptions(ctx);
                rabbit.ConfigureFilter(ctx);

                rabbit.ConfigureEndpoints(ctx);
            });
        });

        return builder;
    }

    private static void ConfigureHost(this IRabbitMqBusFactoryConfigurator cfg, string connection)
    {
        cfg.Host(new Uri(connection), h =>
        {
            h.ConfigureBatchPublish(batch =>
            {
                batch.Enabled = true; // default false
                batch.Timeout = TimeSpan.FromMilliseconds(5); // default 1ms
                batch.MessageLimit = 100; // default
                batch.SizeLimit = 64000; // default
            });
        });
    }
}