using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class AlertModel
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public AlertModel(string type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}