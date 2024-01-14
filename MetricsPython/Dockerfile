# Use the official Python image as the base image
FROM python:3.11-slim

# Set the working directory to /app
WORKDIR /app

# Copy the requirements file into the container at /app
COPY requirements.txt /app/

# Install any dependencies specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Install the ODBC Driver 18 for SQL Server
RUN apt-get update && apt-get install -y gnupg2 curl

# Import the Microsoft GPG key
RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /etc/apt/trusted.gpg.d/microsoft.gpg

# Add the Microsoft SQL Server package repository
RUN curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list

# Update the package list and install the ODBC Driver 18 for SQL Server
RUN apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql18

# Copy the current directory contents into the container at /app
COPY . /app/

# Expose the port on which the Flask app will run
EXPOSE 5000

#CMD ["gunicorn", "-b", "0.0.0.0:5000", "--access-logfile", "-", "manage:app"]
CMD ["manage.py"]