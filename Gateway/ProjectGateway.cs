using RabbitMQ.Client;
using System.Text;

namespace Gateway;

public class ProjectGateway
{
    private IConnection _connection;
    private IModel _channel;
    
    public ProjectGateway()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // _channel.QueueDeclare(queue: "hello",
        // durable: false,
        // exclusive: false,
        // autoDelete: false,
        // arguments: null);
        _channel.ExchangeDeclare(exchange: "project", type: ExchangeType.Fanout);
    }
    public void publish(string args)
    {
        // var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(args);
        _channel.BasicPublish(exchange: "project",
            routingKey: string.Empty,
            basicProperties: null,
            body: body);
        Console.WriteLine($" [x] Sent {args}");
        // var message = "Hello World!";
        // var body = Encoding.UTF8.GetBytes(message);
        //
        // _channel.BasicPublish(exchange: string.Empty,
        // routingKey: "hello",
        // basicProperties: null,
        // body: body);
        // Console.WriteLine($" [x] Sent {message}");
    }
    
    static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
    }
}