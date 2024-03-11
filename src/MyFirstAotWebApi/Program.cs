using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
    new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Calculator Web API, " + builder.Environment.EnvironmentName,
        Version = "v1",
        Description = "Web API example, created with .NET 8",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Filipe Costa",
            Url = new Uri("http://localhost:8123")
        }
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CalculatorAppJsonSerializerContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, EnvironmentJsonSerializerContext.Default);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/healthcheck");
app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc(c =>
{
    c.DocumentTitle = "REDOC API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
});

KeyValue env = new KeyValue("Environment", builder.Environment.EnvironmentName);
KeyValue license = new KeyValue("License", builder.Environment.EnvironmentName);

var calculatorApi = app.MapGroup("/calculator");
calculatorApi.MapGet("/", () => env);
calculatorApi.MapGet("/license", () => env);
calculatorApi.MapGet("/add/{n1}/{n2}", (int n1, int n2) => Results.Ok(n1 + n2));
calculatorApi.MapGet("/sub/{n1}/{n2}", (int n1, int n2) => Results.Ok(n1 - n2));
calculatorApi.MapGet("/mul/{n1}/{n2}", (int n1, int n2) => Results.Ok(n1 * n2));
calculatorApi.MapGet("/div/{n1}/{n2}", (int n1, int n2) => n2 is 0 ? 
    Results.BadRequest("Division by zero is not allowed") : 
    Results.Ok(n1 / n2) );

app.Run();

public record Calculation(int n1, int n2);
[JsonSerializable(typeof(Calculation[]))]
internal partial class CalculatorAppJsonSerializerContext : JsonSerializerContext
{

}

public record KeyValue(string key, string value);
[JsonSerializable(typeof(KeyValue[]))]
internal partial class EnvironmentJsonSerializerContext : JsonSerializerContext
{

}
