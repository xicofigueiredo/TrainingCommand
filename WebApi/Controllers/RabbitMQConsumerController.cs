using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Application.DTO;
using Application.Services;

namespace WebApi.Controllers;

public class RabbitMQConsumerController : IRabbitMQConsumerController
{
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;
    private readonly ProjectService _projectService;
    List<string> _errorMessages = new List<string>();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // public RabbitMQConsumerController()
    // {
    
    public RabbitMQConsumerController(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        string nameProject = "project";
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // _channel.QueueDeclare(queue: "hello",
        //     durable: false,
        //     exclusive: false,
        //     autoDelete: false,
        //     arguments: null);
        _channel.ExchangeDeclare(exchange: nameProject, type: ExchangeType.Fanout);
        
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
            exchange: nameProject,
            routingKey: string.Empty);
        Console.WriteLine(" [*] Waiting for messages.");
    }
    
    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ProjectDTO deserializedObject = ProjectGatewayDTO.ToDTO(message);
            Console.WriteLine($" [x] Received {deserializedObject}");
            Console.WriteLine($" [x] Start checking if exists.");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
                
                ProjectDTO projectResultDTO = projectService.Add(deserializedObject, _errorMessages, false).Result;
            }
        };
        _channel.BasicConsume(queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }
    
    public void StopConsuming()
    {
        
    }
    
    
}