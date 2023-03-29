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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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

        #region Controller的使用

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

            //使用ViewBag
            ViewBag.greet = greet;
            ViewBag.item1 = "鉛筆盒";
            ViewBag.item2 = "水壺";
            ViewBag.item3 = Message;

            //使用ViewData
            ViewData["item4"] = "鑰匙";
            ViewData["item5"] = "手機";

            //使用ViewModel -> 一個頁面只有一個
            User user = new User
            {
                UserId = "A123456789",
                Name = "Max"
            };

            //讀取RouteData
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
        #endregion

        #region 狀態管理
        public IActionResult UseTempData()
        {
            if (TempData["MyTempData"] == null)
            {
                TempData.Add("MyTempData", "[我是使用TempData儲存的資料]");
            }
            return View("StateManagement");
        }

        public IActionResult UseCookie()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("MyCookie");

            CookieOptions options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(7)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("MyCookie", "[我是使用Cookie儲存的資料]");
            return View("StateManagement");
        }

        public IActionResult UseSession()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.Session.Remove("MySession");
            _httpContextAccessor.HttpContext.Session.SetString("MySession", "[我是使用Session儲存的資料]");
            var sessionId = _httpContextAccessor.HttpContext.Session.Id;

            return View("StateManagement");
        }
        #endregion

        #region 自動產生與Tag Helper
        public IActionResult CreateProduct()
        {
            return View();
        }
        #endregion
    }
}
