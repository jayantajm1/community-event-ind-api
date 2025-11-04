namespace CommunityEventsApi.Observability;

public static class OpenTelemetryConfig
{
    public static void ConfigureOpenTelemetry(WebApplicationBuilder builder)
    {
        // TODO: Configure OpenTelemetry for distributed tracing
        // builder.Services.AddOpenTelemetry()
        //     .WithTracing(tracerProviderBuilder =>
        //     {
        //         tracerProviderBuilder
        //             .AddAspNetCoreInstrumentation()
        //             .AddHttpClientInstrumentation()
        //             .AddSqlClientInstrumentation();
        //     });
    }
}
