using Amazon.Runtime;
using Amazon.SQS;
using ContosoEnergy.Core.Interfaces;
using ContosoEnergy.Core.Services;
using ContosoEnergy.Infrastructure.Data;
using ContosoEnergy.Infrastructure.Events;
using ContosoEnergy.Infrastructure.Metrics;
using ContosoEnergy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;       // Distributed tracing
using Serilog;                   // Structured logging
using System.Diagnostics.Metrics; // For custom metrics (Meter/Counter)

// ---------------------------------------------------------------------------
// Application Entry Point (ContosoEnergy.JobService)
// ---------------------------------------------------------------------------
// Configures:
//   - Logging (Serilog structured logs)
//   - Service registration (controllers, Swagger, DI)
//   - Observability (OpenTelemetry tracing + metrics, Prometheus planned)
//   - Middleware pipeline
//   - Application run loop
// ---------------------------------------------------------------------------

var builder = WebApplication.CreateBuilder(args);

#region Logging
/// <summary>
/// Configure <see cref="Serilog"/> as the logging provider.
/// </summary>
/// <remarks>
/// - Reads structured logging configuration from <c>appsettings.json</c>.
/// - Ensures logs are consistent across services.
/// - Related NFRs:
///   <nfr-observability>nfr-06</nfr-observability>
/// </remarks>
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
#endregion

#region Services Registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

#region Custom Metrics
/// <summary>
/// Define custom metrics for business-domain observability.
/// </summary>
/// <remarks>
/// - Jobs Created Counter: tracks number of jobs successfully created.
/// - Related NFRs:
///   <nfr-observability>nfr-06</nfr-observability>
///   <nfr-reliability>nfr-02</nfr-reliability>
/// - Related FRs:
///   <fr-ids>fr-01;fr-02</fr-ids>
/// </remarks>
var jobServiceMeter = new Meter("ContosoEnergy.JobService", "1.0.0");
var jobsCreatedCounter = jobServiceMeter.CreateCounter<int>(
    name: "jobs.created",
    description: "Counts the number of jobs created"
);

// Register custom metric instrument for DI usage
builder.Services.AddSingleton(jobsCreatedCounter);
#endregion

#region OpenTelemetry
/// <summary>
/// Configures OpenTelemetry for distributed tracing and metrics.
/// </summary>
/// <remarks>
/// - Tracing: ASP.NET Core + outgoing HTTP.
/// - Metrics:
///     - Built-in ASP.NET Core & HTTP client metrics.
///     - Custom JobService meter (e.g., jobs.created).
///     - Kestrel & System.Net instrumentation.
///     - Exporters: Prometheus + Console (dev only).
/// - Related NFRs:
///   <nfr-performance>nfr-01</nfr-performance>
///   <nfr-reliability>nfr-02</nfr-reliability>
///   <nfr-scalability>nfr-04</nfr-scalability>
///   <nfr-observability>nfr-06</nfr-observability>
/// </remarks>

// Distributed Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()   // Incoming HTTP request traces
        .AddHttpClientInstrumentation()); // Outgoing HTTP client traces

// OTLP endpoint (if configured for central collector)
var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
var otel = builder.Services.AddOpenTelemetry();

// Resource info (used by exporters like OTLP, Prometheus, etc.)
otel.ConfigureResource(resource => resource
    .AddService(serviceName: builder.Environment.ApplicationName));

// Metrics
otel.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddMeter("ContosoEnergy.JobService")  // Register custom meter
    .AddMeter(jobsCreatedCounter.Name)     // Register jobs.created counter
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddMeter("System.Net.Http")
    .AddMeter("System.Net.NameResolution")
    .AddPrometheusExporter()               // Prometheus scraping endpoint
    .AddConsoleExporter());                // Console exporter (dev only)
#endregion

#region AWS SQS Client
// Load AWS credentials from environment variables
string awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? "test";
string awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY") ?? "test";
string awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";
string localStackUrl = Environment.GetEnvironmentVariable("LOCALSTACK_URL") ?? "http://localhost:4566";

// Create AmazonSQSClient
IAmazonSQS sqsClient = new AmazonSQSClient(
    new BasicAWSCredentials(awsAccessKey, awsSecretKey),
    new AmazonSQSConfig
    {
        ServiceURL = localStackUrl, // LocalStack or actual AWS
        UseHttp = true,
        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
    });

// Register SQS client with DI
builder.Services.AddSingleton(sqsClient);
#endregion

#region Application Services
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IMetricsPublisher, MetricsPublisher>();
builder.Services.AddScoped<IEventSink, SqsEventSink>(x => new SqsEventSink(x.GetService<IAmazonSQS>(), "test-queue-name-for-now!"));
builder.Services.AddScoped<IJobService, TelemetryDomainService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddDbContext<ContosoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("JobDb")));
#endregion


// Build the WebApplication
var app = builder.Build();

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
#endregion

// Run
app.Run();
