package prometheus

import (
	"github.com/gin-gonic/gin"
	"github.com/prometheus/client_golang/prometheus"
	"strconv"
	"time"
)

// Custom metrics setup
var (
	httpRequestDuration = prometheus.NewSummaryVec(prometheus.SummaryOpts{
		Name: "http_request_duration_seconds",
		Help: "Duration of HTTP requests in seconds.",
	}, []string{"method", "status_code", "path"})

	httpRequestCounter = prometheus.NewCounterVec(prometheus.CounterOpts{
		Name: "http_request_total",
		Help: "Total count of HTTP requests",
	}, []string{"method", "status_code", "path"})

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
	prometheus.MustRegister(httpRequestDuration, httpRequestCounter, healthStatus, readinessStatus)
}

func shouldExcludePath(path string) bool {
	excludedPaths := []string{"/", "/favicon.ico", "/metrics", "/health", "/ready"}
	for _, excludedPath := range excludedPaths {
		if path == excludedPath {
			return true
		}
	}
	return false
}

func TrackRequestDurationAndCount() gin.HandlerFunc {
	return func(c *gin.Context) {
		path := c.Request.URL.Path
		if shouldExcludePath(path) {
			c.Next()
			return
		}

		startTime := time.Now()
		c.Next()

		duration := time.Since(startTime).Seconds()
		statusCode := strconv.Itoa(c.Writer.Status())
		method := c.Request.Method

		httpRequestDuration.WithLabelValues(method, statusCode, path).Observe(duration)
		httpRequestCounter.WithLabelValues(method, statusCode, path).Inc()
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
