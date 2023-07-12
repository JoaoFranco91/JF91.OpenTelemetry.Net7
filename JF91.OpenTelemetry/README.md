Use this package to integrate OpenTelemetry into your ASP.NET Web API using ```appsettings.json```

<br>

Follow these steps to get it done:
#### 1: Install Nuget Package
```
dotnet add package JF91.OpenTelemetry --version 1.2.0
```
<br>

#### 2: Add this after ```var builder = WebApplication.CreateBuilder(args);```
```
builder.Services.AddOpenTelemetryServices(builder.Configuration);
```

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