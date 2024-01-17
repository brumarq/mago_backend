using System.Diagnostics;
using System.Security.Claims;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApp.Middleware.Authentication;
using Application.ApplicationServices.Authentization.Interfaces;
using Application.ApplicationServices.Authentization;
using Application.ApplicationServices.Authorization;
using System.Reflection;
using Prometheus;
using WebApp.Middleware.Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<Application.ApplicationServices.Authorization.Interfaces.IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
builder.Services.AddScoped<IAggregatedLogsService, AggregatedLogsService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUsersOnDevicesService, UsersOnDevicesService>();
builder.Services.AddScoped<IDeviceMetricsService, DeviceMetricsService>();
builder.Services.AddScoped<IDeviceAggregatedLogsService, DeviceAggregatedLogsService>();
builder.Services.AddScoped<IApplicationStateService, ApplicationStateService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Device Metrics Orchestrator", Version = "v1" });

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

// Extra stuff for converting int to string representation of enums in Swagger UI.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters
    .Add(new JsonStringEnumConverter()));

var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8585";
builder.WebHost.UseUrls($"http://*:{httpPort}");
builder.Configuration.AddEnvironmentVariables();

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

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseMetricServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
