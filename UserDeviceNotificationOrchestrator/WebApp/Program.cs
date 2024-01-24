using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using Application.ApplicationServices.Interfaces;
using Application.ApplicationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using WebApp.Middleware.Authentication;
using WebApp.Middleware.Prometheus;
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
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<INotificationTokenService, NotificationTokenService>();
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Device User Notifications Orchestrator", Version = "v1" });

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
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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
// Register singleton migration state and custom prometheus metrics
builder.Services.AddSingleton<CustomMetrics>();

builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IApplicationStateService, ApplicationStateService>();

builder.Services.AddHttpContextAccessor();

var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8484";
builder.WebHost.UseUrls($"http://*:{httpPort}");
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// Add custom metric instrumentation for HTTP requests
app.Use(async (context, next) =>
{
     var path = context.Request.Path.Value;
        
        // Normalize paths and exclude /, /health, and /ready
        if (path.Equals("/") || path.Equals("/favicon.ico") || path.Equals("/metrics") || path.Equals("/health") || path.Equals("/ready"))
        {
            await next();
            return; // Skip metrics for these paths
        }
    
    var customMetrics = services.GetRequiredService<CustomMetrics>();

    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var method = context.Request.Method;
    var statusCode = context.Response.StatusCode.ToString();

    // Update metrics
    customMetrics.HttpRequestDuration.WithLabels(method, statusCode, path).Observe(stopwatch.Elapsed.TotalSeconds);
    customMetrics.HttpRequestCounter.WithLabels(method, statusCode, path).Inc();
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseMetricServer(url: "/metrics");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();