using FeeLink.Infrastructure.Common.DependencyInjection;
using FeeLink.Api.Common.DependencyInjection;
using FeeLink.Api.Common.HttpConfigurations;
using FeeLink.Api.WebSockets;
using FeeLink.Application.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();
builder.Services.AddCustomProblemDetails();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddOpenApi(opt => { opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();

// Crear el directorio de datos si no existe
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data");

if (!Directory.Exists(dataPath))
{
    Directory.CreateDirectory(dataPath);
}

await app.UseTriggerSeeder();


app.UseCors(config =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    if (allowedOrigins is null || allowedOrigins.Length == 0)
    {
        throw new InvalidOperationException("CORS config missing");
    }

    config.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
});


app.MapOpenApi();
app.UseSwaggerUI(config => { config.SwaggerEndpoint("/openapi/v1.json", "FeeLink API"); });

app.UseHttpsRedirection();
app.UseGlobalErrorHandling();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();

app.MapSensorDataWS();

app.Run();