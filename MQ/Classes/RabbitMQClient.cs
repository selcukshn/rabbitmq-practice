using System;
using RabbitMQ.Client;

namespace MQ.Classes
{
    public static class RabbitMQClient
    {
        public static string Uri { get; } = "amqp://guest:guest@localhost:5672";

        public static IConnection Connection(string uri)
        {
            return new ConnectionFactory()
            {
                Uri = new Uri(uri, UriKind.RelativeOrAbsolute)
            }
            .CreateConnection();
        }
        public static IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}