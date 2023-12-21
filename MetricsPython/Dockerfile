# Use the official Python image as the base image
FROM python:3.11

# Set the working directory to /app
WORKDIR /app

# Install ODBC driver dependencies
RUN apt-get update && \
    apt-get install -y unixodbc unixodbc-dev

# Copy the requirements file into the container at /app
COPY requirements.txt /app/

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Copy the current directory contents into the container at /app
COPY . /app/

# Expose the port on which the Flask app will run
EXPOSE 5000

# Copy the .env file into the container at /app/ (moved this up)
COPY .env /app/

CMD ["gunicorn", "-b", "0.0.0.0:5000", "manage:app"]