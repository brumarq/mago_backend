from prometheus_client import Gauge

HEALTH_STATUS = Gauge('application_health_status', 'Health status of the application (1 for healthy, 0 for unhealthy)')
READINESS_STATUS = Gauge('application_readiness_status', 'Readiness status of the application (1 for ready, 0 for not ready)')

def set_health_status(is_healthy):
    HEALTH_STATUS.set(1 if is_healthy else 0)

def set_readiness_status(is_ready):
    READINESS_STATUS.set(1 if is_ready else 0)
