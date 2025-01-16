using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrator;
using Orchestrator.Events;
using Orchestrator.StateMachine;

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

app.MapGet("start", async ([FromQuery] long id, [FromServices] ITopicProducer<long, CommunicationEvent> producer) =>
{
    var @event = new CommunicationEvent
    {
        Id = id,
        Text = "Hello World"
    };

    await producer.Produce(@event.Id, @event);
    
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