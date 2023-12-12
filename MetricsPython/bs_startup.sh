# Create virtual environment
python -m venv venv

# Activate virtual environment (for Unix-like systems)
source venv/bin/activate

# Install requirements
pip install -r requirements.txt

# Set Flask app environment variable
export FLASK_APP=manage.py

# Run Flask
flask run