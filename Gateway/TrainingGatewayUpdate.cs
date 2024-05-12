using RabbitMQ.Client;
using System.Text;

namespace Gateway;

public class TrainingGatewayUpdate
{
    private IConnection _connection;
    private IModel _channel;
    private string nameExchange;
    
    public TrainingGatewayUpdate()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        nameExchange = "training_update";

        _channel.ExchangeDeclare(exchange: nameExchange, type: ExchangeType.Fanout);
    }
    public void publish(string args)
    {
        var body = Encoding.UTF8.GetBytes(args);
        _channel.BasicPublish(exchange: nameExchange,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);
        Console.WriteLine($" [x] Sent {args}");

    }
    
}