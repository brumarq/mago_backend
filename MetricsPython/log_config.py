import logging

# Configure the root logger
logging.basicConfig(level=logging.INFO,
                    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')

# Silence the excessive loggers
logging.getLogger('werkzeug').setLevel(logging.INFO)