using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ.Classes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using UI.Enums;
using UI.Extensions;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private static IConnection Connection;
        private static bool ConnectionOpen;
        private IModel _channel;
        private IModel Channel => _channel ?? (_channel = RabbitMQClient.CreateChannel(Connection));
        private static List<string> Logs = new List<string>();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new ConnectionModel()
            {
                Status = ConnectionOpen,
                Logs = Logs
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Connect(string connectionUri)
        {
            if (!ConnectionOpen)
            {
                try
                {
                    Connection = RabbitMQClient.Connection(connectionUri);
                    ConnectionOpen = Connection.IsOpen;
                    Logs.Add("Connection is opened");
                }
                catch (Exception e)
                {
                    Logs.Add(e.Message);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Disconnect(string connectionUri)
        {
            if (ConnectionOpen)
            {
                Connection.Close();
                ConnectionOpen = Connection.IsOpen;
                Logs.Add("Connection is closed");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeclareQueue(string queue)
        {
            if (StringIsInvalid(queue))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid queue name");

            Channel.QueueDeclare(queue, false, false, false, null);
            Logs.Add($"Queue is created [{queue}]");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeclareExchange(string exchange)
        {
            if (StringIsInvalid(exchange))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange name");

            int type;
            if (!int.TryParse(HttpContext.Request.Form["type"], out type))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");

            if (!ExchangeTypeOperation.TypeValid(type))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");

            Channel.ExchangeDeclare(exchange, ExchangeTypeOperation.GetType(type), true);
            Logs.Add($"Exchange is created [{ExchangeTypeOperation.GetType(type)}::{exchange}]");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeclareBind(string exchange, string queue, string key)
        {
            bool isdirect = HttpContext.Request.Form["isdirect"] == "on" ? true : false;
            if (isdirect)
            {
                if (StringIsInvalid(exchange) || StringIsInvalid(queue))
                    return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid queue or exchange");
            }
            else
            {
                if (StringIsInvalid(exchange) || StringIsInvalid(queue) || StringIsInvalid(key))
                    return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid queue,exchange or key");
            }
            try
            {
                if (isdirect)
                {
                    Channel.QueueBind(queue, exchange, queue);
                    Logs.Add($"Bind declared [Exc. : {exchange} - Que. : {queue} with key {queue}]");
                }
                else
                {
                    Channel.QueueBind(queue, exchange, key);
                    Logs.Add($"Bind declared [Exc. : {exchange} - Que. : {queue} with key {key}]");
                }
            }
            catch (Exception e)
            {
                Logs.Add(e.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Publish(string exchange, string key, string body, int repeat)
        {
            if (StringIsInvalid(exchange) || StringIsInvalid(key) || StringIsInvalid(body))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange,key or body");

            int type;
            if (!int.TryParse(HttpContext.Request.Form["type"], out type))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");
            if (!ExchangeTypeOperation.TypeValid(type))
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");

            try
            {
                for (int i = 0; i < repeat; i++)
                {
                    Channel.BasicPublish(exchange, key, null, PrepareBody($"{i} - {body}"));
                }
                Logs.Add("Message published");
            }
            catch (Exception e)
            {
                Logs.Add(e.Message);
            }
            return RedirectToAction("Index");
        }

        public byte[] PrepareBody(string body)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
        }
        public bool StringIsInvalid(string value)
        {
            if (value == null || value == "" || value == " ")
                return true;
            else
                return false;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
