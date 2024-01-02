using System.Security.Claims;
using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApp.Middleware.Authentication;

// using Infrastructure.Data.Context;
// using Infrastructure.Repositories;
// using Infrastructure.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mago - Device Firmware Orchestrator Service", Version = "v1.0.0" });

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


builder.Services.AddHttpClient();
// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<DevicesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevicesDb")));

// Add repositories for dependency injection
// builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddScoped<IFirmwareService, FirmwareService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();
// builder.Services.AddScoped<IDeviceSettingsService, DeviceSettingsService>();

// Add services for dependency injection
//...

builder.Services.AddHttpClient();

var httpPort = Environment.GetEnvironmentVariable("HTTP_PORT") ?? "8383";
builder.WebHost.UseUrls($"http://*:{httpPort}");
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();