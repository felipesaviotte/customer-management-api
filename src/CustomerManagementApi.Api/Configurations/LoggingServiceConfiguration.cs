using CustomerManagementApi.Application.Logs;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace CustomerManagementApi.Api.Configurations;

/// <summary>
/// Configura o serviço de logging.
/// </summary>

/// O Localiza Logging BuildingBlock possui algumas limitações que tornam seu uso menos eficaz em comparação com o Serilog integrado ao DataDog.
/// Como é uma solução corporativa, a implementação permanece no lugar para ser usada no futuro.
[ExcludeFromCodeCoverage]
public static class LoggingServiceConfiguration
{
    /// <summary>
    /// Adiciona o serviço de logging ao contêiner de injeção de dependência.
    /// </summary>
    public static void AddLoggingService(this IServiceCollection services)
    {
        ConfigureSerilog(services);
    }

    private static void ConfigureSerilog(IServiceCollection services)
    {
        LoggingLevelSwitch loggingLevel = new();

        if (Enum.TryParse(Environment.GetEnvironmentVariable("LOG_LEVEL"), true, out LogEventLevel logLevel))
            loggingLevel.MinimumLevel = logLevel;
        else
            loggingLevel.MinimumLevel = LogEventLevel.Information;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(loggingLevel)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .Enrich.FromLogContext()
            .CreateLogger();

        services.AddSingleton<ILoggingService, SerilogLoggingService>();
    }
}
