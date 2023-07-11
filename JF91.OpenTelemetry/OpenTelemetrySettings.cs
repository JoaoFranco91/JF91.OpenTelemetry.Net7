namespace JF91.OpenTelemetry;

public class OpenTelemetrySettings
{
    public bool EnableTraces { get; set; }
    public bool EnableMetrics { get; set; }
    public Instrumentation Instrumentation { get; set; }
    public Exporters Exporters { get; set; }
}

public class Instrumentation
{
    public bool EfCore { get; set; }
    public bool Hangfire { get; set; }
    public bool SqlClient { get; set; }
    public bool Redis { get; set; }
    public bool Http { get; set; }
}

public class Exporters
{
    public Console Console { get; set; }
    public Jaeger Jaeger { get; set; }
    public Zipkin Zipkin { get; set; }
    public Prometheus Prometheus { get; set; }
    public InfluxDB InfluxDB { get; set; }
}

public class ExportersProperties
{
    public bool Enabled { get; set; }
    public string Endpoint { get; set; }
    public string Url { get; set; }
    public string Protocol { get; set; }
}

public static class JaegerProtocols
{
    public static readonly string Http = "http";
    public static readonly string Grpc = "udp";
}

public static class OtlpProtocols
{
    public static readonly string Http = "http";
    public static readonly string Grpc = "grpc";
}

public class Console : ExportersProperties { }
public class Jaeger : ExportersProperties { }

public class Zipkin : ExportersProperties { }

public class Prometheus : ExportersProperties
{
    public string ScrapeEndpointPath { get; set; } = "/metrics";
    public int ScrapeResponseCacheDurationMilliseconds { get; set; } = 300;
}
public class InfluxDB : ExportersProperties { }