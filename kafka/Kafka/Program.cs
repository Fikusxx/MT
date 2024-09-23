using Kafka;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.AddKafka();
builder.Services.AddHostedService<LagService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


// builder.Services.AddSingleton<ISchemaRegistryClient>(provider =>
// {
// 	var schemaRegistryConfig = new SchemaRegistryConfig
// 	{
// 		Url = "https://pkc-w7d6j.germanywestcentral.azure.confluent.cloud:443"
// 	};
//
// 	//return new CachedSchemaRegistryClient(schemaRegistryConfig);
//
// 	return new CachedSchemaRegistryClient(new Dictionary<string, string>
// 			{
// 				{ "schema.registry.url", "pkc-w7d6j.germanywestcentral.azure.confluent.cloud:9092" },
// 				{ "schema.registry.basic.auth.credentials.source", "SASL_INHERIT" },
// 				{ "sasl.username", "qkjuokxx" },
// 				{ "sasl.password", "JDpC2zG-1szh1NtFyNEoTQdss_rIgfcH" }
// 			});
// });

public static unsafe class ObjectExtensions
{
    public static IntPtr GetAddress(this object obj)
    {
        TypedReference tr = __makeref(obj);
        IntPtr ptr = **(IntPtr**)(&tr);
        return ptr;
    }
}