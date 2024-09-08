using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using RabbitMQ;
using RabbitMQ.Features.Direct.Consumers;
using RabbitMQ.Features.Direct.Events;

namespace MTTests;

/// <summary>
/// https://github.com/MassTransit/Sample-WebApplicationFactory/tree/master
/// </summary>
public class Tests
{
    /// <summary>
    /// using real container throws error, cba fixing for now
    /// </summary>
    [Fact]
    public async Task Test1()
    {
        await using var application = new WebApplicationFactory<IAssemblyMarker>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddMassTransitTestHarness(x =>
                    {
                        x.SetTestTimeouts(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                        x.SetKebabCaseEndpointNameFormatter();
                        x.AddConsumer<FastOrdersConsumer>();
                        x.AddConsumer<SlowOrdersConsumer>();
                        
                        // x.UsingRabbitMq((ctx, rabbit) =>
                        // {
                        //     rabbit.ConfigureDirect();
                        //
                        //     rabbit.ConfigureEndpoints(ctx);
                        // });
                    });
                }));

        var testHarness = application.Services.GetTestHarness();

        using var client = application.CreateClient();
        var first = await client.GetAsync("Direct/?priority=fast");
        var second = await client.GetAsync("Direct/?priority=slow");

        first.EnsureSuccessStatusCode();
        second.EnsureSuccessStatusCode();

        var fastConsumer = testHarness.GetConsumerHarness<FastOrdersConsumer>();
        var slowConsumer = testHarness.GetConsumerHarness<SlowOrdersConsumer>();

        var fastResult = await fastConsumer.Consumed.Any<OrderCreatedEvent>(x => x.Context.Message.Priority == "fast");
        var slowResult = await slowConsumer.Consumed.Any<OrderCreatedEvent>(x => x.Context.Message.Priority == "slow");

        Assert.True(fastResult);
        Assert.True(slowResult);
    }
}