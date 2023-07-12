using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

namespace JF91.OpenTelemetry;

public static class OpenTelemetryExtensions
{
    private static OpenTelemetrySettings otelSettings = 
        new OpenTelemetrySettings();
    
    public static IServiceCollection AddOpenTelemetryServices
    (
        this IServiceCollection services,
        IConfiguration config
    )
    {
        config.GetSection(nameof(OpenTelemetrySettings)).Bind(otelSettings);

        services
            .AddOpenTelemetry()
            .WithTracing
            (
                builder =>
                {
                    if (otelSettings.EnableTraces)
                    {
                        builder.AddSource(Environment.GetEnvironmentVariable("APPLICATION_NAME"));
                        builder.ConfigureResource
                        (
                            resource => resource.AddService
                            (
                                Environment.GetEnvironmentVariable("APPLICATION_NAME")
                            )
                        );

                        builder.AddAspNetCoreInstrumentation();

                        if (otelSettings.Instrumentation.Http)
                        {
                            builder.AddHttpClientInstrumentation();
                        }

                        if (otelSettings.Instrumentation.EfCore)
                        {
                            builder.AddEntityFrameworkCoreInstrumentation();
                        }

                        if (otelSettings.Instrumentation.SqlClient)
                        {
                            builder.AddSqlClientInstrumentation();
                        }

                        if (otelSettings.Instrumentation.Redis)
                        {
                            builder.AddRedisInstrumentation();
                        }

                        if (otelSettings.Instrumentation.Hangfire)
                        {
                            builder.AddHangfireInstrumentation();
                        }

                        if (otelSettings.Exporters.Console.Enabled)
                        {
                            builder.AddConsoleExporter();
                        }

                        if (otelSettings.Exporters.Jaeger.Enabled)
                        {
                            builder.AddJaegerExporter
                            (
                                options =>
                                {
                                    options.Endpoint= new Uri(otelSettings.Exporters.Jaeger.Endpoint);
                                    options.Protocol = otelSettings.Exporters.Jaeger.Protocol.ToLower() == JaegerProtocols.Http
                                        ? JaegerExportProtocol.HttpBinaryThrift
                                        : JaegerExportProtocol.UdpCompactThrift;
                                }
                            );
                        }

                        if (otelSettings.Exporters.Zipkin.Enabled)
                        {
                            builder.AddZipkinExporter
                            (
                                options =>
                                {
                                    options.Endpoint = new Uri(otelSettings.Exporters.Zipkin.Endpoint);
                                }
                            );
                        }

                        if (otelSettings.Exporters.InfluxDB.Enabled)
                        {
                            builder.AddOtlpExporter
                            (
                                options =>
                                {
                                    options.Endpoint = new Uri(otelSettings.Exporters.InfluxDB.Url);
                                    options.Protocol = otelSettings.Exporters.InfluxDB.Protocol.ToLower() == OtlpProtocols.Http
                                            ? OtlpExportProtocol.HttpProtobuf
                                            : OtlpExportProtocol.Grpc;
                                }
                            );
                        }

                        if (otelSettings.Exporters.Otlp.Any())
                        {
                            foreach (var otlp in otelSettings.Exporters.Otlp)
                            {
                                if (otlp.Enabled)
                                {
                                    builder.AddOtlpExporter
                                    (
                                        options =>
                                        {
                                            options.Endpoint = new Uri(otlp.Url);
                                            options.Protocol = otlp.Protocol.ToLower() == OtlpProtocols.Http
                                                ? OtlpExportProtocol.HttpProtobuf
                                                : OtlpExportProtocol.Grpc;
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
            )
            .WithMetrics
            (
                builder =>
                {
                    if (otelSettings.EnableMetrics)
                    {
                        builder
                            .AddMeter(Environment.GetEnvironmentVariable("APPLICATION_NAME"))
                            .AddRuntimeInstrumentation()
                            .AddAspNetCoreInstrumentation();
                        
                        if (otelSettings.Exporters.Console.Enabled)
                        {
                            builder.AddConsoleExporter();
                        }
                        
                        if (otelSettings.Instrumentation.Http)
                        {
                            builder.AddHttpClientInstrumentation();
                        }

                        if (otelSettings.Exporters.Prometheus.Enabled)
                        {
                            builder.AddPrometheusExporter
                            (
                                options =>
                                {
                                    options.ScrapeEndpointPath = otelSettings.Exporters.Prometheus.ScrapeEndpointPath;
                                    options.ScrapeResponseCacheDurationMilliseconds = otelSettings.Exporters.Prometheus
                                        .ScrapeResponseCacheDurationMilliseconds;
                                }
                            );
                        }

                        if (otelSettings.Exporters.InfluxDB.Enabled)
                        {
                            builder.AddOtlpExporter
                            (
                                options =>
                                {
                                    // Use IConfiguration directly for Otlp exporter endpoint option.
                                    options.Endpoint = new Uri(otelSettings.Exporters.InfluxDB.Url);
                                    options.Protocol = otelSettings.Exporters.InfluxDB.Protocol.ToLower() == OtlpProtocols.Http
                                        ? OtlpExportProtocol.HttpProtobuf
                                        : OtlpExportProtocol.Grpc;
                                }
                            );
                        }
                        
                        if (otelSettings.Exporters.Otlp.Any())
                        {
                            foreach (var otlp in otelSettings.Exporters.Otlp)
                            {
                                if (otlp.Enabled)
                                {
                                    builder.AddOtlpExporter
                                    (
                                        options =>
                                        {
                                            options.Endpoint = new Uri(otlp.Url);
                                            options.Protocol = otlp.Protocol.ToLower() == OtlpProtocols.Http
                                                ? OtlpExportProtocol.HttpProtobuf
                                                : OtlpExportProtocol.Grpc;
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
            );

        return services;
    }
    
    public static WebApplication AddOpenTelemetryExtensions
    (
        this WebApplication app
    )
    {
        if (otelSettings.Exporters.Prometheus.Enabled)
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
        }

        return app;
    }
}