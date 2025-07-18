using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FeeLink.Infrastructure.Services.Discord;

public class WebhookLogger(HttpClient httpClient, IConfiguration configuration)
{
    private readonly string _webhookUrl = configuration["Discord:WebhookUrl"]!;

    public async Task SendLogAsync(string message, string? title = null, string? color = null, CancellationToken token = default)
    {
        var tagPeople = string.Concat("@everyone\n\n", message);
        
        // Construir el embed con los parámetros
        var embed = new
        {
            title = title ?? "Error Detectado",
            description = tagPeople,
            color = color is not null ? Convert.ToInt32(color, 16) : 16711680,
            timestamp = System.DateTime.UtcNow.ToString("o"),
            footer = new
            {
                text = "\u25c0\ufe0f Logger Bot | Peci \u25b6\ufe0f",
            }
        };

        var payload = new
        {
            username = "Logger",
            embeds = new[] { embed }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(_webhookUrl, content, token);

        response.EnsureSuccessStatusCode();
    }
}
