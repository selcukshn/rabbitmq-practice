using System;
using System.Collections.Generic;
using System.Text;
using MQ.Classes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    class Program
    {
        private static IConnection Connection;
        private static IModel _channel;
        private static IModel Channel => _channel ?? (_channel = RabbitMQClient.CreateChannel(Connection));
        private static bool ConnectionOpen;
        private static List<string> Logs = new List<string>();
        static void Main(string[] args)
        {
        start:
            Console.Write("Enter queue name : ");
            string queueName = Console.ReadLine();
            try
            {
                Connection = RabbitMQClient.Connection(RabbitMQClient.Uri);
                Log("Connection is opened");

                var consumerEvent = new EventingBasicConsumer(Channel);
                consumerEvent.Received += (ch, e) =>
                {
                    var byteArr = e.Body.ToArray();
                    var bodyStr = Encoding.UTF8.GetString(byteArr);
                    Log($"Received data: {bodyStr}");
                };
                Channel.BasicConsume(queueName, true, consumerEvent);
                Log($"{queueName} listening...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log(e.Message);
                goto start;
            }
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
