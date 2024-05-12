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
var trainingQueueName = config["TrainingQueues:" + args[0]];
var trainingUpdateQueueName = config["TrainingUpdateQueues:" + args[0]];

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

builder.Services.AddTransient<ITrainingRepository, TrainingRepository>();
builder.Services.AddTransient<ITrainingFactory, TrainingFactory>();
builder.Services.AddTransient<TrainingMapper>();
builder.Services.AddTransient<TrainingService>();
builder.Services.AddTransient<TrainingGatewayUpdate>();
builder.Services.AddTransient<TrainingGateway>();

builder.Services.AddScoped<TrainingService>();

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
rabbitMQConsumerService.ConfigQueue(trainingQueueName);
rabbitMQConsumerService.StartConsuming();

var rabbitMQConsumerUpdateService = app.Services.GetRequiredService<IRabbitMQConsumerUpdateController>();
rabbitMQConsumerUpdateService.ConfigQueue(trainingUpdateQueueName);
rabbitMQConsumerUpdateService.StartConsuming();

app.MapControllers();

app.Run();