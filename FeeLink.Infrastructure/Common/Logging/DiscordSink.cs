using FeeLink.Infrastructure.Services.Discord;
using Serilog.Core;
using Serilog.Events;

namespace FeeLink.Infrastructure.Common.Logging;

public class DiscordSink : ILogEventSink
{
    private readonly WebhookLogger _webhookLogger;

    public DiscordSink(WebhookLogger webhookLogger)
    {
        _webhookLogger = webhookLogger;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level < LogEventLevel.Error)
            return;

        _ = _webhookLogger.SendLogAsync(message:logEvent.RenderMessage());
    }
}