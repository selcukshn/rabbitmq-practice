using System;
using RabbitMQ.Client;

namespace MQ.Classes
{
    public static class RabbitMQClient
    {
        public static IConnection Connection(string uri)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                Uri = new Uri(uri, UriKind.RelativeOrAbsolute)
            };
            return factory.CreateConnection();
        }
        public static IModel CreateChannel(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}