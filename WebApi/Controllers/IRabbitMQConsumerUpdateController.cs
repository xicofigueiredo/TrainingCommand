namespace WebApi.Controllers;

public interface IRabbitMQConsumerUpdateController
{
    void StartConsuming();
    void ConfigQueue(string queueName);
    void StopConsuming();
}