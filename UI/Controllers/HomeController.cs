using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
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
                Connection = RabbitMQClient.Connection(connectionUri);
                ConnectionOpen = Connection.IsOpen;
                Logs.Add("Connection is opened");
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
                CreateAlert("danger", "Invalid exchange name");
                return RedirectToAction("Index");
            }
            int type;
            if (!int.TryParse(HttpContext.Request.Form["type"], out type))
            {
                CreateAlert("danger", "Invalid exchange type");
                return RedirectToAction("Index");
            }
            if (!ExchangeTypes.TypeValid(type))
            {
                CreateAlert("danger", "Invalid exchange type");
                return RedirectToAction("Index");
            }
            Channel.ExchangeDeclare(exchange, ExchangeTypes.GetType(type));
            Logs.Add($"Exchange is created [{ExchangeTypes.GetType(type)}::{exchange}]");
            return RedirectToAction("Index");
        }

        public void CreateAlert(string type, string message)
        {
            TempData["alert"] = JsonConvert.SerializeObject(new AlertModel(type, message));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
