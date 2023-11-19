using DeviceService.Adapters.SecondaryAdapters.DAL.Repositories;
using DeviceService.Adapters.SecondaryAdapters.DAL.Repositories.Interfaces;
using DeviceService.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add automapper for dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DevicesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevicesDb")));

// Add repositories for dependency injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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