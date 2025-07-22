using System.Net.WebSockets;
using ErrorOr;
using FeeLink.Api.Common.Requests;
using FeeLink.Api.WebSockets;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Common.Wearable;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FeeLink.Api.Common.WebSockets;

public static class WebSocketCommandProcessor
{
    public static async Task<ErrorOr<Success>> ProcessEsp32Save(
        JObject dataToken,
        IMediator mediator,
        IToyRepository repo,
        string identifier,
        WebSocketConnectionManager mgr,
        SensorDataQueue queue)
    {
        // Deserializar DTO
        var dto = dataToken.ToObject<SaveWearableData>()!;

        // Obtener toy y usuarios
        var toy = await repo.GetByMacAsync(identifier);
        if (toy is null)
            return Errors.Toy.NotFound;

        var usersResult = await mediator.Send(new ListLinkedUsersWithToyQuery(toy.Id));
        if (usersResult.IsError)
            return usersResult.Errors;

        var userIds = usersResult.Value.Items.Select(u => u.Id.ToString()).ToList();

        // Encolar para persistencia en background
        var item = new SensorDataQueueItem(toy.Id, dto, identifier, userIds);
        await queue.Queue.Writer.WriteAsync(item);

        // Preparar payload y enviar ACK inmediato
        var payload = new { Message = "Datos recibidos", Sensors = dataToken };
        var jsonSettings = new JsonSerializerSettings
        {
            Converters = { new StringEnumConverter(new CamelCaseNamingStrategy()) }
        };
        var json = JsonConvert.SerializeObject(payload, jsonSettings);

        // Enviar a dispositivo ESP32
        var ws = mgr.GetSocket(identifier);
        if (ws is null || ws.State != WebSocketState.Open)
            return Errors.Toy.TurnedOff;

        await ws.Ok(json);

        // Enviar a cada usuario móvil vinculado
        foreach (var userId in userIds)
        {
            var mobile = mgr.GetSocket(userId);
            if (mobile?.State == WebSocketState.Open)
                await mobile.Ok(json);
        }

        return Result.Success;
    }

    public static async Task<ErrorOr<Success>> SwitchSendingStatus(string userId, JObject dataToken,
        WebSocketConnectionManager mgr, IToyRepository toyRepository)
    {
        // lógica para cambiar el estado de envío...
        var macAddress = dataToken["macAddress"]?.ToString();
        var sendData = dataToken["sendData"]?.ToObject<bool>() ?? false;
        var parsedUserId = Guid.TryParse(userId, out var userGuid) ? userGuid : Guid.Empty;

        if (string.IsNullOrEmpty(macAddress))
            return Errors.Toy.InvalidMacAddress;

        var linkedToys = await toyRepository.ListByUserIdAsync(parsedUserId);

        if (!linkedToys.Items.Any(t => t.MacAddress.Equals(macAddress, StringComparison.OrdinalIgnoreCase)))
            return Errors.Toy.NotLinkedToUser;

        var ws = mgr.GetSocket(macAddress);

        if (ws is null || ws.State != WebSocketState.Open)
            return Errors.Toy.TurnedOff;

        var newCommand =
            new SensorDataWS.WearableCommandRequest<SwitchWearableDataSending>(
                WearableCommandOptions.SwitchSendingStatus,
                new SwitchWearableDataSending { MacAddress = macAddress, SendData = sendData });

        await ws.SendCommand(newCommand);

        return Result.Success;
    }

    public static async Task<ErrorOr<Success>> MobileRequestSendingStatus(JObject dataToken,
        WebSocketConnectionManager mgr)
    {
        var macAddress = dataToken["macAddress"]?.ToString();
        var userId = dataToken["userId"]?.ToString();
        var parsedUserId = Guid.TryParse(userId, out var userGuid) ? userGuid : Guid.Empty;

        if (parsedUserId == Guid.Empty)
            return Errors.User.InvalidId;

        if (string.IsNullOrEmpty(macAddress))
            return Errors.Toy.InvalidMacAddress;

        var payload = new SensorDataWS.WearableCommandRequest<GetWearableSendingStatus>(
            WearableCommandOptions.GetDataSendingStatus,
            new GetWearableSendingStatus { MacAddress = macAddress, UserId = parsedUserId.ToString() });

        var wearableWs = mgr.GetSocket(macAddress);

        if (wearableWs is null || wearableWs.State != WebSocketState.Open)
            return Errors.Toy.TurnedOff;

        await wearableWs.SendCommand(payload);

        return Result.Success;
    }

    public static async Task<ErrorOr<Success>> WearableResponseWithSendingStatus(
        JObject dataToken, WebSocketConnectionManager mgr)
    {
        var userId = dataToken["userId"]?.ToObject<bool>() ?? false;
        var macAddress = dataToken["macAddress"]?.ToString();

        if (string.IsNullOrEmpty(macAddress))
            return Errors.Toy.InvalidMacAddress;

        var payload = new
        {
            Message = "Estado de envío de datos",
            SendingStatus = dataToken["sendingStatus"]?.ToObject<bool>() ?? false
        };
        
        var mobileWs = mgr.GetSocket(userId.ToString());
        
        if (mobileWs is null || mobileWs.State != WebSocketState.Open)
            return Errors.User.NotConnected;
        
        await mobileWs.Ok(JsonConvert.SerializeObject(payload));

        return Result.Success;
    }
    
    public static async Task<ErrorOr<Success>> DisconnectWifi(
        JObject dataToken, WebSocketConnectionManager mgr)
    {
        var macAddress = dataToken["macAddress"]?.ToString();
        
        if (string.IsNullOrEmpty(macAddress))
            return Errors.Toy.InvalidMacAddress;

        var payload = new SensorDataWS.WearableCommandRequest<DisconnectWifi>(
            WearableCommandOptions.DisconnectWifi,
            new DisconnectWifi { MacAddress = macAddress });

        var wearableWs = mgr.GetSocket(macAddress);

        if (wearableWs is null || wearableWs.State != WebSocketState.Open)
            return Errors.Toy.TurnedOff;

        await wearableWs.SendCommand(payload);

        return Result.Success;
    }

    // Extensión para convertir DTO a lista de SensorData
    public static IEnumerable<SensorData> ToSensorData(this SaveWearableData data)
    {
        var list = new List<SensorData>();
        if (data.P?.Pc is float pc) list.Add(new SensorData(pc, Metric.PressurePercent));
        if (data.P?.Gr is float gr) list.Add(new SensorData(gr, Metric.PressureGram));
        if (data.B is float b) list.Add(new SensorData(b, Metric.Battery));
        if (data.M is { } m)
        {
            if (m.X is float x) list.Add(new SensorData(x, Metric.Accel));
            if (m.Y is float y) list.Add(new SensorData(y, Metric.Accel));
            if (m.Z is float z) list.Add(new SensorData(z, Metric.Accel));
        }

        if (data.G is { } g)
        {
            if (g.X is float x) list.Add(new SensorData(x, Metric.Gyro));
            if (g.Y is float y) list.Add(new SensorData(y, Metric.Gyro));
            if (g.Z is float z) list.Add(new SensorData(z, Metric.Gyro));
        }
        
        // Agregar wifi
        if (data.W.HasValue)
            list.Add(new SensorData(data.W.Value ? 1 : 0, Metric.Wifi));

        return list;
    }

    public record SensorData(float Value, Metric Metric);
}