using System.Text;
using Application.DTO;
using Application.Services;
using Domain.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WebApi.Controllers;

public class RabbitMQSagaConsumerController
{
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;
    private readonly TrainingService _trainingService;
    List<string> _errorMessages = new List<string>();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQSagaConsumerController(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        string nameTraining = "training_saga";
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: nameTraining, type: ExchangeType.Fanout);
        
        Console.WriteLine(" [*] Waiting for messages saga.");
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
            exchange: "training_saga",
            routingKey: string.Empty);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            // TrainingDTO deserializedObject = TrainingGatewayDTO.ToDTO(message);
            Console.WriteLine($" [x] Received {message}");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var trainingService = scope.ServiceProvider.GetRequiredService<TrainingService>();

                trainingService.NothingToSeeHereJustReturnOKForTesting();
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
