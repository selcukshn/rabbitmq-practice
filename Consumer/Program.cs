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
        static void Main(string[] args)
        {
        start:
            Console.Write("Enter queue name : ");
            string queueName = Console.ReadLine();
            try
            {
                var connection = RabbitMQClient.Connection(RabbitMQClient.Uri);
                var channel = RabbitMQClient.CreateChannel(connection);

                Log("Connection is opened");

                var consumerEvent = new EventingBasicConsumer(channel);
                consumerEvent.Received += (ch, e) =>
                {
                    var byteArr = e.Body.ToArray();
                    var bodyStr = Encoding.UTF8.GetString(byteArr);
                    Log($"Received data: {bodyStr}");
                };
                channel.BasicConsume(queueName, true, consumerEvent);
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
