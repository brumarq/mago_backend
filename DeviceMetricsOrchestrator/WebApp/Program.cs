using Application.ApplicationServices;
using Application.ApplicationServices.Interfaces;
// using Infrastructure.Data.Context;
// using Infrastructure.Repositories;
// using Infrastructure.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient();
// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDbContext<DevicesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevicesDb")));

// Add repositories for dependency injection
// builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IMetricsService, MetricsService>();
builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();
// builder.Services.AddScoped<IDeviceSettingsService, DeviceSettingsService>();

// Add services for dependency injection
//...

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();