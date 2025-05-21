using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Infrastructure.Services.Esp;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeeLink.Api.WebSockets;

public class SensorData
{
    public int PressurePercent { get; set; }
    public int PressureGrams { get; set; }
    public int Battery { get; set; }
    public (float x, float y, float z) Accelerometer { get; set; }
    public (float x, float y, float z) Gyroscope { get; set; }
    public bool Wifi { get; set; }
    public DateTime Timestamp { get; set; }
}

public static class InMemorySensorDb
{
    public static ConcurrentBag<SensorData> SensorReadings = new();
}

public static class SensorDataWS
{
    public static WebApplication MapSensorDataWS(this WebApplication app)
    {
        app.Map("/ws/sensor-data",
            async (HttpContext context, IMediator mediator, EspJsonReconstructor reconstructor) =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                // Lee el parámetro device de la query string
                var deviceType = context.Request.Query["device"].ToString();

                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024 * 4];

                while (webSocket.State == WebSocketState.Open)
                {
                    var data =
                        await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (data.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client",
                            CancellationToken.None);
                        break;
                    }

                    if (data.MessageType != WebSocketMessageType.Text) continue;

                    var msg = Encoding.UTF8.GetString(buffer, 0, data.Count);

                    if (deviceType == "esp32")
                    {
                        reconstructor.AddFragment(msg);
                        // Procesar datos para esp32
                        if (reconstructor.IsComplete())
                        {
                            var json = reconstructor.GetJson();
                            var command = new SaveSensorDataCommand();
                            var result = await mediator.Send(command);
                            reconstructor.Clear();
                        }
                    }
                    else if (deviceType == "mobile")
                    {
                        // Procesar datos para la app móvil
                    }

                    // Envía OK o tu respuesta normal
                    var okMsg = Encoding.UTF8.GetBytes("OK");
                    await webSocket.SendAsync(new ArraySegment<byte>(okMsg), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
            });

        return app;
    }
}