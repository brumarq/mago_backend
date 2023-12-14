# Use the Microsoft official image with the .NET SDK to build the solution
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy the csproj and restore any dependencies (via nuget)
COPY *.sln .
COPY Application/*.csproj ./Application/
COPY Application.Tests/*.csproj ./Application.Tests/
COPY Domain/*.csproj ./Domain/
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
ENTRYPOINT ["dotnet", "WebApp.dll"]
