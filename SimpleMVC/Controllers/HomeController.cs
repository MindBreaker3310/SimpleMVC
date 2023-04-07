using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleMVC.Filters;
using SimpleMVC.Models;
using SimpleMVC.Repositories;
using SimpleMVC.Services;

namespace SimpleMVC.Controllers
{
    public class HomeController : Controller
    {
        //狀態管理
        private readonly IHttpContextAccessor _httpContextAccessor;
        //Tag Helper
        private ProductsRepository _productsRepository;
        //DI生命週期
        private readonly ISingletonCounter _singletonCounter;
        private readonly IScopedCounter _scopedCounter;
        private readonly ITransientCounter _transientCounter;
        //config & log
        private readonly IConfiguration _configuration;
        private readonly IOptions<MyConfigOptions> _options;
        private readonly ILogger<HomeController> _logger;
        //呼叫外部url用
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger,
            IHttpContextAccessor httpContextAccessor,
            ProductsRepository productsRepository,
            ISingletonCounter singletonCounter,
            IScopedCounter scopedCounter,
            ITransientCounter transientCounter,
            IConfiguration configuration,
            IOptions<MyConfigOptions> options,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _productsRepository = productsRepository;
            _singletonCounter = singletonCounter;
            _scopedCounter = scopedCounter;
            _transientCounter = transientCounter;
            _configuration = configuration;
            _options = options;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
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

        #region Tag Helper & Tag Helper Attributes
        public IActionResult UseFormTagHelperAttributes()
        {
            return View();
        }

        [Route("Home/ShowAllProducts", Name = "ShowAll")]
        public IActionResult ShowAllProducts()
        {
            return View(_productsRepository.Products);
        }

        public IActionResult ProductDetail(int id)
        {
            return View(_productsRepository.Products.FirstOrDefault(x => x.ProductId == id));
        }

        public IActionResult ProductCreate()
        {
            ViewBag.newId = _productsRepository.Products.Max(x => x.ProductId) + 1;
            return View();
        }

        //預設是全部綁定，要只綁定ProductId,ProductName兩個參數，其他都不用的方法如下
        //public IActionResult ProductCreate([Bind("ProductId,ProductName")]Product product)
        [HttpPost]
        public IActionResult ProductCreate(Product product)
        {
            //模型都符合data annotation
            if (ModelState.IsValid)
            {
                _productsRepository.Add(product);
                return Redirect("ShowAllProducts");
            }
            else
            {
                ViewBag.newId = _productsRepository.Products.Max(x => x.ProductId) + 1;
                return View();
            }
        }

        public IActionResult ProductEdit(int id)
        {
            return View(_productsRepository.Products.FirstOrDefault(x => x.ProductId == id));
        }

        [HttpPost]
        public IActionResult ProductEdit(Product product)
        {
            //模型都符合data annotation
            if (ModelState.IsValid)
            {
                _productsRepository.Update(product);
                return View("ShowAllProducts", _productsRepository.Products);
            }
            return View(_productsRepository.Products.FirstOrDefault(x => x.ProductId == product.ProductId));
        }

        public IActionResult ProductDelete(int id)
        {
            _productsRepository.Delete(id);
            return View("ShowAllProducts", _productsRepository.Products);
        }
        #endregion

        #region 依賴注入
        public IActionResult CounterPlusOne()
        {
            ViewBag.SingletonCount = _singletonCounter.GetCounter();
            ViewBag.ScopedCount = _scopedCounter.GetCounter();
            ViewBag.TransientCount = _transientCounter.GetCounter();

            return View("VisitCounter");
        }
        #endregion

        #region Config檔與Log記錄
        public IActionResult ConfigAndLog()
        {
            ViewBag.LogInfo = _configuration["Logging:LogLevel:Default"];

            ViewBag.MySetting1 = _options.Value.MySetting1;
            ViewBag.MySetting2 = _options.Value.MySetting2;
            ViewBag.MySetting3 = _options.Value.MySetting3;

            _logger.LogTrace("我是Trace");
            _logger.LogDebug("我是Debug");
            _logger.LogInformation("我是Information");
            _logger.LogWarning("我是Warning");
            _logger.LogError("我是Error");
            _logger.LogCritical("我是Critical");

            return View();
        }
        #endregion

        #region 過濾器
        [MyCustomFilter]//可以放action上，也可以放在controller上
        public IActionResult UseFilter()
        {
            _httpContextAccessor.HttpContext.Response.WriteAsync("<br />--------- View --------- <br />");
            return Content("");
        }

        [MyAsyncCustomFilter]
        public IActionResult UseAsyncFilter()
        {
            _httpContextAccessor.HttpContext.Response.WriteAsync("<br />--------- View --------- <br />");
            return Content("");
        }

        [MyCustomFilter]
        [MyAsyncCustomFilter]
        public IActionResult UseAsyncAndNonAsyncFilter()
        {
            _httpContextAccessor.HttpContext.Response.WriteAsync("<br />--------- View --------- <br />");
            return Content("");
        }

        [ServiceFilter(typeof(MyCustomDIFilter))]
        public IActionResult UseMyCustomDIFilter()
        {
            _httpContextAccessor.HttpContext.Response.WriteAsync("<br />--------- View --------- <br />");
            return Content("");
        }

        [ServiceFilter(typeof(MyCustomExceptionFilter))]
        public IActionResult UseMyCustomExceptionFilter()
        {
            int one = 1;
            int zero = 0;
            int bad = one / zero;//會報錯
            return Content("");
        }
        #endregion

        #region identity Server 4驗證

        public async Task<IActionResult> IdentityServerConfig()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(_configuration["IdentityServerConfigUrl"]);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "text/json");
        }

        public async Task<IActionResult> IdentityServerUserToken()
        {
            var httpClient = _httpClientFactory.CreateClient();

            //建立x-www-form-unlencoded物件
            var postData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", "User"),
                new KeyValuePair<string, string>("client_secret", "userSecret"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            var payload = new FormUrlEncodedContent(postData);

            var response = await httpClient.PostAsync(_configuration["IdentityServerTokenUrl"], payload);
            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            var accessToken = json["access_token"].ToString();

            //存放cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("MyAccessTokenCookie");

            CookieOptions options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(7)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("MyAccessTokenCookie", accessToken);

            return Content(content, "text/json");
        }

        public async Task<IActionResult> IdentityServerAdminToken()
        {
            var httpClient = _httpClientFactory.CreateClient();

            //建立x-www-form-unlencoded物件
            var postData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", "Admin"),
                new KeyValuePair<string, string>("client_secret", "adminSecret"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            var payload = new FormUrlEncodedContent(postData);

            var response = await httpClient.PostAsync(_configuration["IdentityServerTokenUrl"], payload);
            var content = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(content);
            var accessToken = json["access_token"].ToString();

            //存放cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("MyAccessTokenCookie");

            CookieOptions options = new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(7)
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("MyAccessTokenCookie", accessToken);

            return Content(content, "text/json");
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminGet()
        {
            return Content("Yes, only Admin can accrss this API");
        }

        [Authorize(Roles = "user")]
        public IActionResult UserGet()
        {
            return Content("Yes, user can accrss this API");
        }
        #endregion
    }
}

