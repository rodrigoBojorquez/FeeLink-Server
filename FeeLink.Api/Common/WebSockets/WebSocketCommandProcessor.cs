using System.Net.WebSockets;
using ErrorOr;
using FeeLink.Api.Common.Requests;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Application.UseCases.Sensors.Commands.SaveData;
using FeeLink.Application.UseCases.Users.Queries.ListLinkedWithToy;
using FeeLink.Domain.Common.Errors;
using FeeLink.Domain.Entities;
using FeeLink.Infrastructure.Services.WebSocket;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FeeLink.Api.Common.WebSockets;

public static class WebSocketCommandProcessor
{
    public static async Task<ErrorOr<Success>> ProcessEsp32Save(
        JObject dataToken,
        IMediator mediator,
        IToyRepository repo,
        string identifier,
        WebSocketConnectionManager mgr)
    {
        var dto = dataToken.ToObject<SaveWearableData>()!;
        var toy = await repo.GetByMacAsync(identifier);
        if (toy is null) 
            return Errors.Toy.NotFound;

        var usersResult = await mediator.Send(new ListLinkedUsersWithToyQuery(toy.Id));
        
        if (usersResult.IsError)
            return usersResult.Errors;

        var sensorDataList = dto.ToSensorData().ToList();

        foreach (var sensor in sensorDataList)
        {
            var saveResult = await mediator.Send(new SaveSensorDataCommand(toy.Id, sensor.Value, sensor.Metric));
            if (saveResult.IsError) 
                return saveResult.Errors.First();
        }

        // Prepara un único payload
        var payload = new 
        {
            Message = "Datos guardados correctamente",
            Sensors = sensorDataList
        };
        
        var jsonSettings = new JsonSerializerSettings
        {
            Converters = {new StringEnumConverter()}
        };
        
        var json = JsonConvert.SerializeObject(payload, jsonSettings);

        // Enviar una sola vez al dispositivo
        var ws = mgr.GetSocket(identifier);
        if (ws is null || ws.State != WebSocketState.Open) 
            return Errors.Toy.TurnedOff;
        await ws.Ok(json);

        // Enviar una sola vez a cada usuario móvil vinculado
        var userIds = usersResult.Value.Items.Select(u => u.Id.ToString());
        foreach (var userId in userIds)
        {
            var mobile = mgr.GetSocket(userId);
            if (mobile?.State == WebSocketState.Open)
                await mobile.Ok(json);
        }

        return Result.Success;
    }

    public static Task<ErrorOr<Success>> ProcessEsp32Start(JObject dataToken)
    {
        // lógica para Start...
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }

    public static Task<ErrorOr<Success>> ProcessMobileRegister(JObject dataToken, WebSocketConnectionManager mgr)
    {
        // lógica para Register...
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }

    public static Task<ErrorOr<Success>> ProcessMobileNotify(JObject dataToken)
    {
        // lógica para Notify...
        return Task.FromResult<ErrorOr<Success>>(Result.Success);
    }
    
    // Extensión para convertir DTO a lista de SensorData
    private static IEnumerable<SensorData> ToSensorData(this SaveWearableData data)
    {
        var list = new List<SensorData>();
        if (data.P?.Pc is float pc) list.Add(new SensorData(pc, Metric.PressurePercent));
        if (data.P?.Gr is float gr) list.Add(new SensorData(gr, Metric.PressureGram));
        if (data.B is float b)      list.Add(new SensorData(b, Metric.Battery));
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
        return list;
    }

    public record SensorData(float Value, Metric Metric);
}