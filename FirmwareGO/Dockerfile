# Start from a base golang image
FROM golang:1.21.1

# Set the Current Working Directory inside the container
WORKDIR /app

# Copy src code
COPY . .

# Download all the dependencies.
RUN go mod download

# Install & run swag command
RUN go install github.com/swaggo/swag/cmd/swag@v1.16.2
RUN swag init

# Build the Go app
RUN go build -o main .

# Expose port 6969 to outside world
EXPOSE 6969

# Run the executable
CMD ["./main"]