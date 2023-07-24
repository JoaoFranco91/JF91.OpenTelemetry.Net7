using JF91.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryServices(builder.Configuration);

// builder.Services.AddOpenTelemetryServices
// (
//     builder.Configuration,
//     jaegerOptions =>
//     {
//         jaegerOptions.Enabled = true;
//         jaegerOptions.Endpoint = "http://www.test.com";
//         jaegerOptions.Protocol = JaegerProtocols.Http;
//     },
//     zipkinOptions =>
//     {
//         zipkinOptions.Enabled = true;
//         zipkinOptions.Endpoint = "http://www.test.com";
//     },
//     influxdbOptions =>
//     {
//         influxdbOptions.Enabled = true;
//         influxdbOptions.Url = "http://www.test.com";
//         influxdbOptions.Protocol = JaegerProtocols.Http;
//     },
//     new List<Action<Otlp>>
//     {
//         otlp_one =>
//         {
//             otlp_one.Enabled = true;
//             otlp_one.Url = "http://www.test.com";
//             otlp_one.Protocol = OtlpProtocols.Http;
//         },
//         otlp_two =>
//         {
//             otlp_two.Enabled = true;
//             otlp_two.Url = "http://www.test2.com";
//             otlp_two.Protocol = OtlpProtocols.Grpc;
//         }
//     },
//     prometheusOptions =>
//     {
//         prometheusOptions.Enabled = true;
//         prometheusOptions.ScrapeEndpointPath = "/test";
//         prometheusOptions.ScrapeResponseCacheDurationMilliseconds = 1000;
//     }
// );

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.AddOpenTelemetryExtensions();

app.Run();