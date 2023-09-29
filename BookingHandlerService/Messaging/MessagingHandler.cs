using System.Text;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;

namespace BookingHandlerService.Messaging
{
    public class MessagingHandler
    {
        public MessagingHandler() 
        {
            
        }
        public void SendDTO()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            const string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
