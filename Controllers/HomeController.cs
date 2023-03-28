using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleMVC.Models;

namespace SimpleMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Action1()
        {
            return View("Privacy");
        }

        public string Action2()
        {
            return "Action2 回傳string";
        }

        public IActionResult Action3()
        {
            return Json(new { data = "data123" });
        }

        public IActionResult Action4(string Message = "Good Good Good")
        {
            int hours = DateTime.Now.Hour;
            string greet = hours < 12 ? "上午好!" : "下午好!";


            ViewBag.greet = greet;
            ViewBag.item1 = "鉛筆盒";
            ViewBag.item2 = "水壺";
            ViewBag.item3 = Message;

            User user = new User
            {
                UserId = "A123456789",
                Name = "Max"
            };

            var controller = RouteData.Values["Controller"];
            var action = RouteData.Values["Action"];
            var id = RouteData.Values["Id"];

            ViewBag.RouteData = @$"
                Controller = {controller},
                Action = {action},
                Id = {id}";

            return View("Action4", user);
        }


        [HttpPost]
        public IActionResult Action5(IFormCollection ifc)
        {
            string url = ifc["url"];

            if (string.IsNullOrEmpty(url))
            {
                return Redirect("https://www.google.com");
            }
            return Redirect(url);
        }

        public IActionResult Action6()
        {
            string viewModel = "PartialView";
            return PartialView("Action6", viewModel);
        }
    }
}
