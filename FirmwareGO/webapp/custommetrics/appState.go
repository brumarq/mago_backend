package custommetrics

import "github.com/prometheus/client_golang/prometheus"

var (
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
	prometheus.MustRegister(healthStatus, readinessStatus)
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
