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
var projectQueueName = config["ProjectQueues:" + args[0]];
var projectUpdateQueueName = config["ProjectUpdateQueues:" + args[0]];

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AbsanteeContext>(opt =>
    //opt.UseInMemoryDatabase("AbsanteeList")
    //opt.UseSqlite("Data Source=AbsanteeDatabase.sqlite")
    opt.UseSqlite(Host.CreateApplicationBuilder().Configuration.GetConnectionString("AbsanteeDatabase"))
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
// builder.Services.AddTransient<IColaboratorRepository, ColaboratorRepository>();
// builder.Services.AddTransient<IColaboratorFactory, ColaboratorFactory>();
// builder.Services.AddTransient<ColaboratorMapper>();
// builder.Services.AddTransient<ColaboratorService>();
//
// builder.Services.AddTransient<IHolidayPeriodRepository, HolidayPeriodRepository>();
// builder.Services.AddTransient<IHolidayPeriodFactory, HolidayPeriodFactory>();
// builder.Services.AddTransient<HolidayPeriodMapper>();
// builder.Services.AddTransient<HolidayPeriodService>();
//
// builder.Services.AddTransient<IHolidayRepository, HolidayRepository>();
// builder.Services.AddTransient<IHolidayFactory, HolidayFactory>();
// builder.Services.AddTransient<HolidayMapper>();
// builder.Services.AddTransient<HolidayService>();
//
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddTransient<IProjectFactory, ProjectFactory>();
builder.Services.AddTransient<ProjectMapper>();
// builder.Services.AddTransient<ProjectService>();
builder.Services.AddTransient<ProjectGateway>();
builder.Services.AddTransient<ProjectGatewayUpdate>();
builder.Services.AddTransient<ProjectAMQPService>();

builder.Services.AddScoped<ProjectService>();
// builder.Services.AddScoped<RabbitMQConsumerController>();

builder.Services.AddSingleton<IRabbitMQConsumerController, RabbitMQConsumerController>();
builder.Services.AddSingleton<IRabbitMQConsumerUpdateController, RabbitMQConsumerUpdateController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

app.UseAuthorization();
var rabbitMQConsumerService = app.Services.GetRequiredService<IRabbitMQConsumerController>();
rabbitMQConsumerService.ConfigQueue(projectQueueName);
rabbitMQConsumerService.StartConsuming();

var rabbitMQConsumerUpdateService = app.Services.GetRequiredService<IRabbitMQConsumerUpdateController>();
rabbitMQConsumerUpdateService.ConfigQueue(projectUpdateQueueName);
rabbitMQConsumerUpdateService.StartConsuming();

app.MapControllers();

app.Run();