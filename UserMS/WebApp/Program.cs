using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using WebApp.Middleware.Authentication;
using System.Diagnostics;
using WebApp.Middleware.Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// AutoMapper configuration for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - User Service...", Version = "v1" });

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

builder.Services.AddSingleton<IAuth0ManagementService, Auth0ManagementService>();
builder.Services.AddScoped<IAuth0Service, Auth0Service>();
builder.Services.AddScoped<IAuth0RolesService, Auth0RolesService>();
builder.Services.AddScoped<IApplicationStateService, ApplicationStateService>();

builder.Services.AddHttpClient();

var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{httpPort}");
builder.Configuration.AddEnvironmentVariables();

// Build the application
var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// Add custom metric instrumentation for HTTP requests
app.Use(async (context, next) =>
{
    var customMetrics = services.GetRequiredService<CustomMetrics>();

    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var method = context.Request.Method;
    var statusCode = context.Response.StatusCode.ToString();

    // Update metrics
    customMetrics.HttpRequestDuration.WithLabels(method, statusCode).Observe(stopwatch.Elapsed.TotalSeconds);
    customMetrics.HttpRequestCounter.WithLabels(method, statusCode).Inc();
});

// Middleware configuration
app.UseMetricServer();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
