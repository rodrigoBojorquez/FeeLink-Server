using System.Net.WebSockets;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FeeLink.Api.WebSockets;

public static class SensorDataWS
{
    public static WebApplication Add(this WebApplication app)
    {
        app.Map("/ws/sensor-data", async (HttpContext context, IMediator mediator) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                // Pendiente agregar la implementacion
                // var command = new Command();
                // await mediator.Send();
                    
                var okMsg = Encoding.UTF8.GetBytes("OK");
                await webSocket.SendAsync(new ArraySegment<byte>(okMsg), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        });
        return app;
    }
}