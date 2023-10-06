using System.Text;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using BookingHandlerService.Models;
using System.Text.Json;

namespace BookingHandlerService.Messaging
{
    public class MessagingHandler
    {
        public MessagingHandler() 
        {
        }

        public static void SendDTO(PlanDTO planDTO, string path)
        {
            var factory = new ConnectionFactory { HostName = path };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "plan",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string planDTOJson = JsonSerializer.Serialize(planDTO);
            var body = Encoding.UTF8.GetBytes(planDTOJson);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: "plan",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
