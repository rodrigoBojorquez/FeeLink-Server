using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
using ErrorOr;
using FeeLink.Api.Common.WebSockets;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace FeeLink.Api.WebSockets;

public static class SensorDataWS
{
    public record WebSocketRequest(string Cmd, SensorDataPayload Data);

    public class SensorDataPayload
    {
        [JsonPropertyName("p")] public Pressure? P { get; set; }

        [JsonPropertyName("b")] public float? B { get; set; }

        [JsonPropertyName("m")] public AxisData? M { get; set; }

        [JsonPropertyName("g")] public AxisData? G { get; set; }

        [JsonPropertyName("w")] public bool? W { get; set; }
    }

    public class Pressure
    {
        [JsonPropertyName("pc")] public float? Pc { get; set; }

        [JsonPropertyName("gr")] public float? Gr { get; set; }
    }

    public class AxisData
    {
        [JsonPropertyName("x")] public float? X { get; set; }

        [JsonPropertyName("y")] public float? Y { get; set; }

        [JsonPropertyName("z")] public float? Z { get; set; }
    }

    public record SensorData(float Value, Metric Metric);

    public enum SensorDataCommand
    {
        Start,
        Stop,
        Save
    }

    public static WebApplication MapSensorDataWS(this WebApplication app)
    {
        app.Map("/ws/sensor-data",
            async (HttpContext context, IMediator mediator, WebSocketConnectionManager socketManager,
                IToyRepository toyRepository) =>
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
                        socketManager.RemoveSocket(identifier);
                        break;
                    }

                    if (data.MessageType != WebSocketMessageType.Text)
                    {
                        await webSocket.Problem([
                            Error.Validation("WebSocket.InvalidMessageType",
                                "El tipo de mensaje recibido no es válido. Se esperaba un mensaje de texto.")
                        ]);
                        continue;
                    }


                    var msg = Encoding.UTF8.GetString(buffer, 0, data.Count);
                    var deserializedRequest = DeserializeWebSocketRequest(msg);

                    if (deserializedRequest.IsError)
                    {
                        await webSocket.Problem(deserializedRequest.Errors);
                        continue;
                    }

                    var nodeCommand = deserializedRequest.Value;

                    switch (deviceType)
                    {
                        case "esp32":
                        {
                            if (IsValidCommand(nodeCommand.Cmd, SensorDataCommand.Save))
                            {
                                var toy = await toyRepository.GetByMacAsync(identifier);
                                if (toy is null)
                                {
                                    await webSocket.Problem([
                                        Error.NotFound("Toy.NotFound",
                                            "No se encontró el juguete con el identificador proporcionado.")
                                    ]);
                                    continue;
                                }

                                // Encontrar WS de tipo "mobile" correspondientes
                                var linkedUsersQuery = new ListLinkedUsersWithToyQuery(toy.Id);
                                var linkedUsersResult = await mediator.Send(linkedUsersQuery);

                                if (linkedUsersResult.IsError)
                                    await webSocket.Problem(linkedUsersResult.Errors);

                                var sensorDataList = GetSensorData(nodeCommand.Data);
                                var linkedUsersIds = linkedUsersResult.Value.Items
                                    .Select(u => u.Id)
                                    .ToList();

                                foreach (var sensorData in sensorDataList)
                                {
                                    var saveDataCommand = new SaveSensorDataCommand(
                                        toy.Id,
                                        sensorData.Value,
                                        sensorData.Metric
                                    );

                                    var saveResult = await mediator.Send(saveDataCommand);

                                    if (saveResult.IsError)
                                    {
                                        await webSocket.Problem(saveResult.Errors);
                                        continue;
                                    }

                                    // Enviar respuesta al dispositivo ESP32
                                    await webSocket.Ok("Datos guardados correctamente");

                                    // Enviar datos a los usuarios móviles vinculados
                                    foreach (var userId in linkedUsersIds)
                                    {
                                        var mobileSocket = socketManager.GetSocket(userId.ToString());
                                        if (mobileSocket is { State: WebSocketState.Open })
                                        {
                                            await mobileSocket.Ok(JsonConvert.SerializeObject(sensorData));
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        case "mobile":
                        {
                            var request = JsonConvert.DeserializeObject<WebSocketRequest>(msg);
                            if (request is null || string.IsNullOrWhiteSpace(request.Cmd)) break;

                            switch (request.Cmd.ToLower())
                            {
                                case "open":
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            });

        return app;
    }

    private static bool IsValidCommand(string command, SensorDataCommand commandType)
    {
        return Enum.IsDefined(typeof(SensorDataCommand), commandType);
    }

    private static ErrorOr<WebSocketRequest> DeserializeWebSocketRequest(string message)
    {
        try
        {
            var request = JsonConvert.DeserializeObject<WebSocketRequest>(message);
            return request is not null
                ? request
                : Error.Validation("WebSocket.InvalidRequest", "Formato de solicitud WebSocket no válido");
        }
        catch (JsonException)
        {
            return Error.Validation("WebSocket.InvalidRequest", $"Error al deserializar la solicitud WebSocket");
        }
    }

    private static List<SensorData> GetSensorData(SensorDataPayload data)
    {
        var sensorData = new List<SensorData>();

        if (data.P?.Pc is float pcValue)
            sensorData.Add(new SensorData(pcValue, Metric.PressurePercent));

        if (data.P?.Gr is float grValue)
            sensorData.Add(new SensorData(grValue, Metric.PressureGram));

        if (data.B is float battery)
            sensorData.Add(new SensorData(battery, Metric.Battery));

        if (data.M is not null)
        {
            if (data.M.X is float x) sensorData.Add(new SensorData(x, Metric.Accel));
            if (data.M.Y is float y) sensorData.Add(new SensorData(y, Metric.Accel));
            if (data.M.Z is float z) sensorData.Add(new SensorData(z, Metric.Accel));
        }

        if (data.G is not null)
        {
            if (data.G.X is float x) sensorData.Add(new SensorData(x, Metric.Gyro));
            if (data.G.Y is float y) sensorData.Add(new SensorData(y, Metric.Gyro));
            if (data.G.Z is float z) sensorData.Add(new SensorData(z, Metric.Gyro));
        }

        if (data.W is bool w)
            sensorData.Add(new SensorData(w ? 1f : 0f, Metric.Wifi));

        return sensorData;
    }
}