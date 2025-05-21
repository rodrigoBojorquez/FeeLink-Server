namespace FeeLink.Infrastructure.Services.WebSocket;

public class WebSocketConnectionManager
{
    private readonly Dictionary<string, System.Net.WebSockets.WebSocket> _sockets = new();
    private readonly object _lock = new();

    public void AddSocket(string identifier, System.Net.WebSockets.WebSocket socket)
    {
        lock (_lock) { _sockets[identifier] = socket; }
    }

    public System.Net.WebSockets.WebSocket? GetSocket(string mac)
    {
        lock (_lock) { _sockets.TryGetValue(mac, out var ws); return ws; }
    }

    public void RemoveSocket(string identifier)
    {
        lock (_lock) { _sockets.Remove(identifier); }
    }

    public IReadOnlyDictionary<string, System.Net.WebSockets.WebSocket> GetAllSockets()
    {
        lock (_lock) { return new Dictionary<string, System.Net.WebSockets.WebSocket>(_sockets); }
    }
}
