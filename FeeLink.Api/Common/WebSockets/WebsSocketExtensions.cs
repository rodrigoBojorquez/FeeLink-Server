using System.Net.WebSockets;
using System.Text;
using ErrorOr;

namespace FeeLink.Api.Common.WebSockets;

public static class WebsSocketExtensions
{
    public static async Task Problem(this WebSocket webSocket, List<Error> errors, CancellationToken cancellationToken = default)
    {
        if (errors.Count == 0)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "No errors", cancellationToken);
            return;
        }

        var errorMessage = string.Join(", ", errors.Select(e => e.Description));
        var message = Encoding.UTF8.GetBytes(errorMessage);
        
        await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, cancellationToken);
    }
    
    public static async Task Ok(this WebSocket webSocket, string message, CancellationToken cancellationToken = default)
    {
        var responseMessage = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(responseMessage), WebSocketMessageType.Text, true, cancellationToken);
    }
}