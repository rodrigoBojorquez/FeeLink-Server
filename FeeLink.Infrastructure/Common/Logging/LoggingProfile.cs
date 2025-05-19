using Microsoft.Extensions.Configuration;
using Serilog;

namespace FeeLink.Infrastructure.Common.Logging;

public static class LoggingProfile
{
    public static void Configure(IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
    } 
}