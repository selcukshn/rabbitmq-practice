using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ.Classes;
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
        private static IModel Channel => RabbitMQClient.CreateChannel(Connection);
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
        public IActionResult CreateQueue(string connectionUri)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CreateExchange(string exchange)
        {
            if (string.IsNullOrEmpty(exchange) || string.IsNullOrWhiteSpace(exchange))
            {
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange name");
            }
            int type;
            if (!int.TryParse(HttpContext.Request.Form["type"], out type))
            {
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");
            }
            if (!ExchangeTypeOperation.TypeValid(type))
            {
                return this.RedirectToActionAlert("Index", EAlertType.danger, "Invalid exchange type");
            }

            Channel.ExchangeDeclare(exchange, ExchangeTypeOperation.GetType(type));
            Logs.Add($"Exchange is created [{ExchangeTypeOperation.GetType(type)}::{exchange}]");
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
