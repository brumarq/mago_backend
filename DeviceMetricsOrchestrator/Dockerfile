# Use the Microsoft official image with the .NET SDK to build the solution
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

EXPOSE 8585
ENV ASPNETCORE_URLS=http://0.0.0.0:8585

# Copy the csproj and restore any dependencies (via nuget)
COPY *.sln .
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Domain/*.csproj ./Domain/
COPY Domain.Tests/*.csproj ./Domain.Tests/
COPY WebApp/*.csproj ./WebApp/

RUN dotnet nuget locals all --clear
RUN dotnet restore

# Copy the project files and build the release
COPY . ./
RUN dotnet build -c Release

# Publish the application
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Create a user with a specific user ID (e.g., 1001) and switch to it
# Ensure this user ID has the necessary permissions to run your application
RUN useradd -u 1001 -r -g 0 -d /app -s /sbin/nologin -c "ASP.NET Core User" dotnetuser && \
    chown -R 1001:0 /app && \
    chmod -R g=u /app
USER 1001

ENTRYPOINT ["dotnet", "WebApp.dll"]
