using System.Diagnostics;
using Prometheus;

namespace WebApp.Middleware.prometheus;

public class CustomMetrics
{
    // Custom Prometheus metrics for ready and health checks
    public Gauge HealthCheckGauge { get; } = Metrics.CreateGauge(
        "application_health_status",
        "Health status of the application (1 for healthy, 0 for unhealthy)"
    );

    public Gauge ReadinessCheckGauge { get; } = Metrics.CreateGauge(
        "application_readiness_status",
        "Readiness status of the application (1 for healthy, 0 for unhealthy)"
    );
    
    // Custom Prometheus metrics for HTTP requests
    public Histogram HttpRequestDuration { get; } = Metrics.CreateHistogram(
        "http_request_duration_seconds",
        "Duration of HTTP requests in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "method", "status_code", "path" }
        }
    );

    public Counter HttpRequestCounter { get; } = Metrics.CreateCounter(
        "http_request_total",
        "Total count of HTTP requests",
        new CounterConfiguration
        {
            LabelNames = new[] { "method", "status_code", "path" }
        }
    );

    // Custom Metric for process resident memory in bytes
    public Gauge ProcessResidentMemoryBytes { get; } = Metrics.CreateGauge(
        "process_resident_memory_bytes",
        "Resident memory size of the process in bytes"
    );

    public CustomMetrics()
    {
        ProcessResidentMemoryBytes.Set(Process.GetCurrentProcess().WorkingSet64);
    }
}