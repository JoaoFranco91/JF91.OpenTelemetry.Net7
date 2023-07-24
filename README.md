Use this package to integrate OpenTelemetry into your ASP.NET Web API using ```appsettings.json```

<br>

Follow these steps to get it done:
#### 1: Install Nuget Package
```
dotnet add package JF91.OpenTelemetry
```
<br>

#### 2: Add this after ```var builder = WebApplication.CreateBuilder(args);```
```
builder.Services.AddOpenTelemetryServices(builder.Configuration);
```

#### 2.2: Customize settings (Optional):
###### Custom options are optional which means that you can mix options from appsettings.json with the ones defined below
```
builder.Services.AddOpenTelemetryServices
(
    builder.Configuration,
    jaegerOptions =>
    {
        jaegerOptions.Enabled = true;
        jaegerOptions.Endpoint = "http://www.test.com";
        jaegerOptions.Protocol = JaegerProtocols.Http;
    },
    zipkinOptions =>
    {
        zipkinOptions.Enabled = true;
        zipkinOptions.Endpoint = "http://www.test.com";
    },
    influxdbOptions =>
    {
        influxdbOptions.Enabled = true;
        influxdbOptions.Url = "http://www.test.com";
        influxdbOptions.Protocol = JaegerProtocols.Http;
    },
    new List<Action<Otlp>>
    {
        otlp_one =>
        {
            otlp_one.Enabled = true;
            otlp_one.Url = "http://www.test.com";
            otlp_one.Protocol = OtlpProtocols.Http;
        },
        otlp_two =>
        {
            otlp_two.Enabled = true;
            otlp_two.Url = "http://www.test2.com";
            otlp_two.Protocol = OtlpProtocols.Grpc;
        }
    },
    prometheusOptions =>
    {
        prometheusOptions.Enabled = true;
        prometheusOptions.ScrapeEndpointPath = "/test";
        prometheusOptions.ScrapeResponseCacheDurationMilliseconds = 1000;
    }
);
```
#### IMPORTANT: For a custom options list to OTLP Exporters, you need to have the same ammount of entries in appsettings.json with the same order.

Example 1: 3 OTLP Exporters in appsettings.json and 2 Custom Options will override the first 2 OTLP Exportes in appsettings.json from the first 2 Custom Options.

Example 2: 2 OTLP Exporters in appsettings.json and 3 Custom Options will override the 2 OTLP Exporters in appsettings.json from the first 2 Custom Options.

<br>

#### 3: Add this before ```app.Run();```
```
app.AddOpenTelemetryExtensions();
```

<br>

#### 4: Add this to your ```appsettings.json``` and modify it to your needs:
```
"OpenTelemetrySettings": {
    "EnableTraces": true,
    "EnableMetrics": true,
    "Exporters": {
        "Console": {
            "Enabled": true
        },
        "Jaeger": {
            "Enabled": true,
            "Endpoint": "http://localhost:14268/api/traces",
            "Protocol": "udp"
        },
        "Zipkin": {
            "Enabled": false,
            "Endpoint": " https://localhost:9411/api/v2/spans"
        },
        "Prometheus": {
            "Enabled": true,
            "ScrapeEndpointPath": "/metrics-text",
            "ScrapeResponseCacheDurationMilliseconds": 300
        },
        "InfluxDB": {
            "Enabled": false,
            "Url": "http://localhost:8086",
            "Protocol": "http"
        },
        "Otlp": [
        {
          "Enabled": true,
          "Url": "http://localhost:4317",
          "Protocol": "grpc"
        },
        {
          "Enabled": false,
          "Url": "http://localhost:4317",
          "Protocol": "http"
        }
      ]
    },
    "Instrumentation": {
        "Http": true,
        "EfCore": false,
        "Hangfire": false,
        "SqlClient": false,
        "Redis": false
    }
}
```

### Properties Values:
- Jaeger.Protocol: udp / http | Default => udp

- [OtlpProtocols] > InfluxDB.Protocol: grpc / http | Default => grpc

#### Reference: https://opentelemetry.io/docs/instrumentation/net/exporters/
