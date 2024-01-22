using System.Diagnostics;
using System.Reflection;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Prometheus;
using WebApp.Middleware.Authentication;
using WebApp.Middleware.prometheus;
using WebApp.Middleware.status;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add automapper for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DevicesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevicesDb")));

// Add repositories for dependency injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add services for dependency injection
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthorizationsService, AuthorizationsService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();
builder.Services.AddScoped<IDeviceSettingsService, DeviceSettingsService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUsersOnDevicesService, UsersOnDevicesService>();
builder.Services.AddScoped<IApplicationStateService, ApplicationStateService>();
// Register singleton migration state and custom prometheus metrics
builder.Services.AddSingleton<MigrationStatus>();
builder.Services.AddSingleton<CustomMetrics>();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Device Service", Version = "v1.0.0" });

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

builder.Configuration.AddEnvironmentVariables();
var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8181";
builder.WebHost.UseUrls($"http://*:{httpPort}");

var app = builder.Build();


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

// Apply pending migrations on boot-up
var context = services.GetRequiredService<DevicesDbContext>();
var migrationStatus = services.GetRequiredService<MigrationStatus>();

try
{
    context.Database.Migrate();
    // Applying migration succeeds --> set status to true
    migrationStatus.SetMigrationStatus(true);
}
catch (Exception e)
{
    // Applying migration fails --> set status to false
    migrationStatus.SetMigrationStatus(false);
    Console.WriteLine(e.Message);
}

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
app.UseMetricServer();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();