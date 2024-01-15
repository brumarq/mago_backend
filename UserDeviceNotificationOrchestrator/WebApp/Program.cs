using System.Diagnostics;
using System.Security.Claims;
using Application.ApplicationServices.Interfaces;
using Application.ApplicationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using WebApp.Middleware.Authentication;
using IAuthorizationService = Application.ApplicationServices.Interfaces.IAuthorizationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services for dependency injection
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient();

// Create custom Prometheus metrics for HTTP requests
var httpRequestDuration = Metrics.CreateHistogram(
    "http_request_duration_seconds",
    "Duration of HTTP requests in seconds",
    new HistogramConfiguration
    {
        LabelNames = new[] { "method", "status_code" }
    }
);
var httpRequestCounter = Metrics.CreateCounter(
    "http_request_total",
    "Total count of HTTP requests",
    new CounterConfiguration
    {
        LabelNames = new[] { "method", "status_code" }
    }
);

// Create a gauge metric for process resident memory in bytes
var processResidentMemoryBytes = Metrics.CreateGauge(
    "process_resident_memory_bytes",
    "Resident memory size of the process in bytes"
);
processResidentMemoryBytes.Set(Process.GetCurrentProcess().WorkingSet64);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Device Metrics Orchestrator", Version = "v1.0.0" });

    // Security schema for Swagger UI
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});

// Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Domain"];
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("permissions", "admin"));
    options.AddPolicy("Client", policy => policy.RequireClaim("permissions", "client"));
    options.AddPolicy("All", policy => policy.RequireAssertion(context =>
        context.User.HasClaim(c => 
            (c.Type == "permissions" && (c.Value == "admin" || c.Value == "client")))));
});

// Authorization handler registration
builder.Services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();

builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpContextAccessor();


var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8484";
builder.WebHost.UseUrls($"http://*:{httpPort}");
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Add custom metric instrumentation for HTTP requests
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var method = context.Request.Method;
    var statusCode = context.Response.StatusCode.ToString();

    // Update metrics
    httpRequestDuration
        .WithLabels(method, statusCode)
        .Observe(stopwatch.Elapsed.TotalSeconds);

    httpRequestCounter
        .WithLabels(method, statusCode)
        .Inc();
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseMetricServer(url: "/metrics");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();