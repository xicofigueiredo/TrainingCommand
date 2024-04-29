using Microsoft.EntityFrameworkCore;

using Application.Services;
using DataModel.Repository;
using DataModel.Mapper;
using Domain.Factory;
using Domain.IRepository;
using Gateway;
using WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var connectionString = config.GetConnectionString("AbsanteeDatabase" + args[0]);
var projectQueueName = config["ProjectQueues:" + args[0]];
var projectUpdateQueueName = config["ProjectUpdateQueues:" + args[0]];

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    opt.UseSqlite(connectionString)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddTransient<IProjectFactory, ProjectFactory>();
builder.Services.AddTransient<ProjectMapper>();
builder.Services.AddTransient<ProjectService>();
builder.Services.AddTransient<ProjectGatewayUpdate>();
builder.Services.AddTransient<ProjectGateway>();

builder.Services.AddScoped<ProjectService>();

builder.Services.AddSingleton<IRabbitMQConsumerController, RabbitMQConsumerController>();
builder.Services.AddSingleton<IRabbitMQConsumerUpdateController, RabbitMQConsumerUpdateController>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseRouting();

app.UseCors("AllowAllOrigins"); 

app.UseAuthorization();

var rabbitMQConsumerService = app.Services.GetRequiredService<IRabbitMQConsumerController>();
rabbitMQConsumerService.ConfigQueue(projectQueueName);
rabbitMQConsumerService.StartConsuming();

var rabbitMQConsumerUpdateService = app.Services.GetRequiredService<IRabbitMQConsumerUpdateController>();
rabbitMQConsumerUpdateService.ConfigQueue(projectUpdateQueueName);
rabbitMQConsumerUpdateService.StartConsuming();

app.MapControllers();

app.Run();