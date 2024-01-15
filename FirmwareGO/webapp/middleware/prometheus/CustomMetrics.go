package prometheus

import (
	"github.com/gin-gonic/gin"
	"github.com/prometheus/client_golang/prometheus"
	"time"
)

// Custom metrics setup
var (
	httpRequestDuration = prometheus.NewSummary(prometheus.SummaryOpts{
		Name: "http_request_duration_seconds",
		Help: "Duration of HTTP requests in seconds.",
	})

	healthStatus = prometheus.NewGauge(prometheus.GaugeOpts{
		Name: "application_health_status",
		Help: "Health status of the application (1 for healthy, 0 for unhealthy)",
	})

	readinessStatus = prometheus.NewGauge(prometheus.GaugeOpts{
		Name: "application_readiness_status",
		Help: "Readiness status of the application (1 for ready, 0 for not ready)",
	})
)

func init() {
	prometheus.MustRegister(httpRequestDuration, healthStatus, readinessStatus)
}

func TrackRequestDuration() gin.HandlerFunc {
	return func(c *gin.Context) {
		startTime := time.Now()
		c.Next()

		duration := time.Since(startTime).Seconds()
		httpRequestDuration.Observe(duration)
	}
}

func SetHealthStatus(isHealthy bool) {
	if isHealthy {
		healthStatus.Set(1) // Set to 1 for healthy
	} else {
		healthStatus.Set(0) // Set to 0 for unhealthy
	}
}

func SetReadinessStatus(isReady bool) {
	if isReady {
		readinessStatus.Set(1) // Set to 1 for ready
	} else {
		readinessStatus.Set(0) // Set to 0 for not ready
	}
}
