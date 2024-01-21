from prometheus_client import Gauge, Histogram, Counter
import psutil

"""
Methods for setting health and readiness status | Application state tracking
"""
# Customer Prometheus gauges for keeping track of health and readiness
HEALTH_STATUS = Gauge('application_health_status', 'Health status of the application (1 for healthy, 0 for unhealthy)')
READINESS_STATUS = Gauge('application_readiness_status', 'Readiness status of the application (1 for ready, 0 for not ready)')

# Custom Prometheus metrics for HTTP requests
HTTP_REQUEST_DURATION = Histogram('http_request_duration_seconds', 'Duration of HTTP requests in seconds', labelnames=['method', 'status_code'])
HTTP_REQUEST_COUNTER = Counter('http_request_total', 'Total count of HTTP requests', labelnames=['method', 'status_code'])

PROCESS_RESIDENT_MEMORY_BYTES = Gauge(
    'process_resident_memory_bytes',
    'Resident memory size of the process in bytes'
)

def update_resident_memory_bytes():
    process = psutil.Process()
    resident_memory_bytes = process.memory_info().rss
    PROCESS_RESIDENT_MEMORY_BYTES.set(resident_memory_bytes)
    
update_resident_memory_bytes()

# Methods for setting health and readiness status
def set_health_status(is_healthy):
    HEALTH_STATUS.set(1 if is_healthy else 0)

def set_readiness_status(is_ready):
    READINESS_STATUS.set(1 if is_ready else 0)