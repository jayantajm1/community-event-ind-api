namespace CommunityEventsApi.Observability;

public static class LoggingConfig
{
    public static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // TODO: Add structured logging (e.g., Serilog)
        // builder.Host.UseSerilog((context, services, configuration) => configuration
        //     .ReadFrom.Configuration(context.Configuration)
        //     .Enrich.FromLogContext()
        //     .WriteTo.Console());
    }
}
