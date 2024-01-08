FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HealthCheckApp/HealthCheckApp.csproj", "HealthCheckApp/"]
RUN dotnet restore "HealthCheckApp/HealthCheckApp.csproj"
COPY . .
WORKDIR "/src/HealthCheckApp"
RUN dotnet build "HealthCheckApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HealthCheckApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthCheckApp.dll"]
