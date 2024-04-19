using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Application.DTO;
using Application.Services;

namespace WebApi.Controllers;

public class RabbitMQConsumerUpdateController : IRabbitMQConsumerUpdateController
{
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;
    private readonly ProjectService _projectService;
    List<string> _errorMessages = new List<string>();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQConsumerUpdateController(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        string nameProject = "project_update";
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: nameProject, type: ExchangeType.Fanout);
        
        // _queueName = _channel.QueueDeclare().QueueName;
        // _channel.QueueBind(queue: _queueName,
        //     exchange: nameProject,
        //     routingKey: string.Empty);
        Console.WriteLine(" [*] Waiting for messages update.");
    }
    
    public void ConfigQueue(string queueName)
    {
        _queueName = queueName;

        _channel.QueueDeclare(queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(queue: _queueName,
            exchange: "project_update",
            routingKey: string.Empty);
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
            Console.WriteLine($" [x] Start updating.");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
                
                bool result = projectService.Update(deserializedObject.Id, deserializedObject, _errorMessages, false).Result;
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