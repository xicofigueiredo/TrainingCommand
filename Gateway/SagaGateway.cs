using System.Text;
using RabbitMQ.Client;

namespace Gateway;

public class SagaGateway
{
    private IConnection _connection;
    private IModel _channel;
    private string nameExchange;

    public SagaGateway()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        nameExchange = "project_saga";
        
        _channel.ExchangeDeclare(exchange: nameExchange, type: ExchangeType.Fanout);
    }

    public void publish(string args)
    {
        var body = Encoding.UTF8.GetBytes(args);
        _channel.BasicPublish(exchange: nameExchange,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);
        Console.WriteLine(" [x] Sent OK");
    }
}