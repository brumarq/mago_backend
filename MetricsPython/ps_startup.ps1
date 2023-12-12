# Create virtual environment
python -m venv venv

# Activate virtual environment (for Windows)
.\venv\Scripts\activate

# Install requirements
pip install -r requirements.txt

# Set Flask app environment variable
$env:FLASK_APP = "manage.py"

# Run Flask
flask run