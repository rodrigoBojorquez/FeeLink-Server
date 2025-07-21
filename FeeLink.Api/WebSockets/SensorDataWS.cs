using System.Net.WebSockets;
using System.Text;
using ErrorOr;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Infrastructure.Common.Wearable;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FeeLink.Api.WebSockets;

public static class SensorDataWS
{
    public record WearableCommandRequest<T>(WearableCommandOptions Cmd, T Data);

    public static WebApplication MapSensorDataWS(this WebApplication app)
    {
        app.Map("/ws/sensor-data",
            async (HttpContext context, IMediator mediator, WebSocketConnectionManager socketManager,
                IToyRepository toyRepository, SensorDataQueue sensorDataQueue) =>
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

                // Define para cada tipo de dispositivo sus comandos y procesadores
                var deviceHandlers = new Dictionary<string, Func<string, Task>>
                {
                    ["esp32"] = msg => HandleDeviceCommands(
                        msg,
                        webSocket,
                        new Dictionary<WearableCommandOptions, Func<JObject, Task<ErrorOr<Success>>>>
                        {
                            [WearableCommandOptions.Save] = data =>
                                WebSocketCommandProcessor.ProcessEsp32Save(data, mediator, toyRepository, identifier,
                                    socketManager, sensorDataQueue),
                            [WearableCommandOptions.GetDataSendingStatus] = data =>
                                WebSocketCommandProcessor.WearableResponseWithSendingStatus(data, socketManager)
                        }
                    ),
                    ["mobile"] = msg => HandleDeviceCommands(
                        msg,
                        webSocket,
                        new Dictionary<WearableCommandOptions, Func<JObject, Task<ErrorOr<Success>>>>
                        {
                            [WearableCommandOptions.SwitchSendingStatus] = data =>
                                WebSocketCommandProcessor.SwitchSendingStatus(identifier, data, socketManager,
                                    toyRepository),
                            [WearableCommandOptions.GetDataSendingStatus] = data =>
                                WebSocketCommandProcessor.MobileRequestSendingStatus(data, socketManager)
                        }
                    )
                };

                if (!deviceHandlers.TryGetValue(deviceType, out var handler))
                {
                    await webSocket.Problem([
                        Error.Validation("WebSocket.InvalidDevice", $"Dispositivo '{deviceType}' no soportado")
                    ]);
                    return;
                }

                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client",
                            CancellationToken.None);
                        socketManager.RemoveSocket(identifier);
                        break;
                    }

                    if (result.MessageType != WebSocketMessageType.Text)
                    {
                        await webSocket.Problem([
                            Error.Validation("WebSocket.InvalidMessageType", "Se esperaba mensaje de texto")
                        ]);
                        continue;
                    }

                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await handler(msg);
                }
            });

        return app;
    }

    // Deserializa el JObject, valida el comando y dispatch al procesador
    private static async Task HandleDeviceCommands(
        string msg,
        WebSocket ws,
        Dictionary<WearableCommandOptions, Func<JObject, Task<ErrorOr<Success>>>> handlers)
    {
        WearableCommandRequest<JObject>? baseReq;
        try
        {
            baseReq = JsonConvert.DeserializeObject<WearableCommandRequest<JObject>>(msg)
                      ?? throw new JsonException();
        }
        catch
        {
            await ws.Problem([Error.Validation("WebSocket.InvalidRequest", "Formato inválido")]);
            return;
        }

        if (!handlers.TryGetValue(baseReq.Cmd, out var action))
        {
            await ws.Problem([Error.Validation("WebSocket.InvalidCommand", $"Comando '{baseReq.Cmd}' no soportado")]);
            return;
        }

        var result = await action(baseReq.Data);

        if (result.IsError)
            await ws.Problem(result.Errors);
    }
}