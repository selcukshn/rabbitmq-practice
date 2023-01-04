using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UI.Enums;
using UI.Models;

namespace UI.Extensions
{
    public static class ControllerExtension
    {
        public static IActionResult RedirectToActionAlert(this Controller c, string action, EAlertType type, string message)
        {
            c.TempData["alert"] = CreateAlert(type, message);
            return c.RedirectToAction(action);
        }
        public static IActionResult RedirectToActionAlert(this Controller c, string action, string controller, EAlertType type, string message)
        {
            c.TempData["alert"] = CreateAlert(type, message);
            return c.RedirectToAction(action, controller);
        }



        public static string CreateAlert(EAlertType type, string message)
        {
            return JsonConvert.SerializeObject(new AlertModel(type, message));
        }
    }
}