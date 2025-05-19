using System.Net.WebSockets;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeeLink.Api.WebSockets;

public static class SensorDataWS
{
    public static WebApplication MapSensorDataWS(this WebApplication app)
    {
        app.Map("/ws/sensor-data", async (HttpContext context, IMediator mediator) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024 * 4];

                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Procesa datos si quieres
                        var okMsg = Encoding.UTF8.GetBytes("OK");
                        await webSocket.SendAsync(new ArraySegment<byte>(okMsg), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        });

        return app;
    }
}