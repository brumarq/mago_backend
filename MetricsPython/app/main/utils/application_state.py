from prometheus_client import Gauge, Counter, Summary
from flask import request
import time
from threading import active_count

"""
Methods for setting health and readiness status | Application state tracking
"""
# Customer Prometheus gauges for keeping track of health and readiness
HEALTH_STATUS = Gauge('application_health_status', 'Health status of the application (1 for healthy, 0 for unhealthy)')
READINESS_STATUS = Gauge('application_readiness_status', 'Readiness status of the application (1 for ready, 0 for not ready)')

# Custom Prometheus metrics for HTTP requests
HTTP_REQUEST_DURATION = Summary('http_request_duration_seconds', 'Duration of HTTP requests in seconds', labelnames=['method', 'status_code', 'path'])
HTTP_REQUEST_COUNTER = Counter('http_request_total', 'Total count of HTTP requests', labelnames=['method', 'status_code', 'path'])
THREAD_COUNT = Gauge('process_num_threads', 'Number of active threads in the application')


def __should_exclude_path(path):
    excluded_paths = ['/', '/favicon.ico', '/health', '/ready', '/swagger.json'] # full paths
    
    if path == '/metrics': # metrics tracking path
        return True
    
    if path.startswith('/swaggerui'): #anythign that start with /swaggerui
        return True
    
    for excluded_path in excluded_paths:
         if path == excluded_path:
            return True
        
    return False

def track_request_duration_and_count(response):
    path = request.path
    if __should_exclude_path(path):
        return
    
    duration = time.time() - request.start_time if hasattr(request, 'start_time') else 0

    status_code = str(response.status_code)
    method = request.method

    HTTP_REQUEST_DURATION.labels(method=method, status_code=status_code, path=path).observe(duration)
    HTTP_REQUEST_COUNTER.labels(method=method, status_code=status_code, path=path).inc()
    THREAD_COUNT.set(active_count())


# Methods for setting health and readiness status
def set_health_status(is_healthy):
    HEALTH_STATUS.set(1 if is_healthy else 0)

def set_readiness_status(is_ready):
    READINESS_STATUS.set(1 if is_ready else 0)