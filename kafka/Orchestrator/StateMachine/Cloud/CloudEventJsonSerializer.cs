using System.Net.Mime;
using System.Text;
using System.Text.Json;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.SystemTextJson;
using Confluent.Kafka;
using Orchestrator.Events;

namespace Orchestrator.StateMachine.Cloud;

/// <summary>
/// https://github.com/cloudevents/sdk-csharp
/// </summary>
internal sealed class CloudEventJsonSerializer : IAsyncSerializer<CloudEvent>
{
    private static readonly JsonEventFormatter Formatter = new();

    public Task<byte[]> SerializeAsync(CloudEvent data, SerializationContext context)
    {
        var element = Formatter.ConvertToJsonElement(data);
        var serializedData = JsonSerializer.Serialize(element);
        return Task.FromResult(Encoding.UTF8.GetBytes(serializedData));
    }
}

internal sealed class CommunicationCloudEventJsonDeserializer : BaseCloudEventJsonSerializer<CommunicationEvent>, IDeserializer<CommunicationEvent>
{
    public CommunicationEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var result = BaseDeserialize(data);

        return result as CommunicationEvent ?? throw new InvalidOperationException("CloudEvent data is not CommunicationEvent");
    }
}

internal abstract class BaseCloudEventJsonSerializer<T> where T : class
{
    private readonly ContentType contentType = new(MediaTypeNames.Application.Json);
    private readonly JsonEventFormatter<T> formatter = new();

    protected object? BaseDeserialize(ReadOnlySpan<byte> data)
    {
        var wrapper = formatter.DecodeStructuredModeMessage(new MemoryStream(data.ToArray()), contentType, null);
        return wrapper.Data;
    }
}