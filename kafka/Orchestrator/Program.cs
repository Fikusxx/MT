using System.Net.Mime;
using CloudNative.CloudEvents;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrator;
using Orchestrator.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransport(builder.Configuration);

var app = builder.Build();

var sp = builder.Services.BuildServiceProvider();
var db = sp.GetService<DbContext>();
await db.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("start", async ([FromQuery] long id,
    // [FromServices] ITopicProducer<long, CommunicationEvent> producer,
    [FromServices] ITopicProducer<long, CloudEvent> cloudProducer,
    [FromServices] ITopicProducerProvider provider) =>
{
    var @event = new CommunicationEvent
    {
        Id = id,
        Text = "Hello World"
    };

    var cloudEvent = new CloudEvent
    {
        Id = Guid.NewGuid().ToString(),
        Type = "Type Name",
        Source = new Uri("https://cloudevents.io/"),
        Time = DateTimeOffset.UtcNow,
        DataContentType = MediaTypeNames.Application.Json,
        Data = @event
    };

    // var cloudEventProducer = provider.GetProducer<long, CloudEvent>(new Uri("topic:start"));
    // var cloudEventProducer = provider.GetProducer<long, CommunicationEvent>(new Uri("topic:start"));
    // await cloudEventProducer.Produce(id, cloudEvent);
    // await producer.Produce(@event.Id, @event);
    // await cloudProducer.Produce(id, cloudEvent);
    await cloudProducer.Produce(id, cloudEvent);

    return Results.Ok();
});

app.MapGet("push-status", async ([FromQuery] long id, [FromServices] ITopicProducer<long, PushStatusEvent> producer) =>
{
    var @event = new PushStatusEvent
    {
        Id = id,
        Success = true
    };

    await producer.Produce(@event.Id, @event);

    return Results.Ok();
});


app.Run();


public static unsafe class ObjectExtensions
{
    public static IntPtr GetAddress(this object obj)
    {
        TypedReference tr = __makeref(obj);
        IntPtr ptr = **(IntPtr**)(&tr);
        return ptr;
    }
}