using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Infrastructure.Services.Esp;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeeLink.Api.WebSockets;

public static class SensorDataWS
{
    public static WebApplication MapSensorDataWS(this WebApplication app)
    {
        app.Map("/ws/sensor-data",
            async (HttpContext context, IMediator mediator, EspJsonReconstructor reconstructor, WebSocketConnectionManager socketManager) =>
            {
                if (!context.WebSockets.IsWebSocketRequest) return;

                var deviceType = context.Request.Query["device"].ToString();
                var identifier = context.Request.Query["identifier"].ToString();
                
                if (string.IsNullOrEmpty(deviceType) || string.IsNullOrEmpty(identifier))
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Dispositivo o identificador no proporcionado");
                    return;
                }
                
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                socketManager.AddSocket(identifier, webSocket);
                
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

                    switch (deviceType)
                    {
                        case "esp32":
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

                            break;
                        }
                        case "mobile":
                        {
                            // Procesar datos para la app móvil
                            var command = new SaveSensorDataCommand();
                            var result = await mediator.Send(command);
                            break;
                        }
                    }
                }
            });

        return app;
    }
}