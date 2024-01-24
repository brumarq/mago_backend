import logging
from logging.config import dictConfig

# Configure the logging settings
log_config = {
    'version': 1,
    'disable_existing_loggers': False,
    'handlers': {
        'stdout': {
            'class': 'logging.StreamHandler',
            'stream': 'ext://sys.stdout',
            'formatter': 'default',
        },
    },
    'formatters': {
        'default': {
            'format': '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
        },
    },
    'root': {
        'level': 'INFO',
        'handlers': ['stdout'],
    },
    'loggers': {
        'werkzeug': {
            'level': 'INFO',
            'handlers': ['stdout'],
            'propagate': False,
        },
    },
}

dictConfig(log_config)