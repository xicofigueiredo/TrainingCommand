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
    private string nameTraining;
    private readonly TrainingService _trainingService;
    List<string> _errorMessages = new List<string>();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQConsumerUpdateController(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        nameTraining = "training_update";
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: nameTraining, type: ExchangeType.Fanout);
        
        // _queueName = _channel.QueueDeclare().QueueName;
        // _channel.QueueBind(queue: _queueName,
        //     exchange: nameTraining,
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
            exchange: nameTraining,
            routingKey: string.Empty);
    }
    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            TrainingDTO deserializedObject = TrainingGatewayDTO.ToDTO(message);
            Console.WriteLine($" [x] Received {deserializedObject}");
            Console.WriteLine($" [x] Start updating.");
            
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var trainingService = scope.ServiceProvider.GetRequiredService<TrainingService>();
                
                bool result = trainingService.Update(deserializedObject.Id, deserializedObject, _errorMessages, false).Result;
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