# Start from a base golang image
FROM golang:1.21.1

# Set the Current Working Directory inside the container
WORKDIR /app

# Copy src code
COPY . .

# Download all the dependencies
RUN go mod download

# Build the Go app
RUN go build -o main .

# Expose port 8080 to outside world
EXPOSE 8080

# Run the executable
CMD ["./main"]