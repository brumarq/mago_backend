using System.Diagnostics;
using System.Security.Claims;
using Application.ApplicationServices.Interfaces;
using Application.ApplicationServices;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApp.Middleware.Authentication;
using Prometheus;
using IAuthorizationService = Application.ApplicationServices.Interfaces.IAuthorizationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add automapper for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotificationsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationsDb")));

// Add repositories for dependency injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add services for dependency injection
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Notification Service", Version = "v1.0.0" });

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

builder.Configuration.AddEnvironmentVariables();
var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8282";
builder.WebHost.UseUrls($"http://*:{httpPort}");

// Migration on boot-up
var serviceProvider = builder.Services.BuildServiceProvider();
var context = serviceProvider.GetRequiredService<NotificationsDbContext>();
context.Database.Migrate();

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

app.UseMetricServer(url: "/metrics");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
