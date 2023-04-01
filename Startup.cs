using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleMVC.Filters;
using SimpleMVC.Middlewares;
using SimpleMVC.Repositories;
using SimpleMVC.Services;

namespace SimpleMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            //將TempData改使用Session儲存
            //services.AddControllersWithViews().AddSessionStateTempDataProvider();
            //services.AddSession();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //使用session
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;//可以從用cookie把session回復起來
            });

            //新增Views搜尋資料夾範圍
            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Add("~/Views/MyCustomShared/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("~/Views/Home/TagHelper/{0}" + RazorViewEngine.ViewExtension);
            });

            //測試資料
            services.AddSingleton<ProductsRepository>();

            //DI不同的生命週期
            services.AddSingleton<ISingletonCounter, SingletonCounter>();
            services.AddScoped<IScopedCounter, ScopedCounter>();
            services.AddTransient<ITransientCounter, TransientCounter>();

            //config使用option pattern
            services.Configure<MyConfigOptions>(Configuration.GetSection("MyConfig"));

            //註冊過濾器
            services.AddScoped<MyCustomDIFilter>();
            services.AddScoped<MyCustomExceptionFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            //建置自定義Middleware(使用匿名函數)
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethod.Get.ToString() && context.Request.Query["CallMyMiddleware"] == "true")
                {
                    await context.Response.WriteAsync("Greeting from my middleware\n\r");
                }
                await next();
            });
            //建置自定義Middleware(使用外部class)(兩種方法，擇一使用)
            app.UseMiddleware<MyMiddleware>();
            app.UseMyMiddleware();


            //使用分支功能>如果url是/branch，多執行一些middleware
            app.Map("/branch", (branch) =>
            {
                branch.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("Branch Use \n\r");
                    await next();
                });

                //使用Run將不會跑到下一個middleware
                branch.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Branch Run \n\r");
                });
            });




            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async (context) =>
                {
                    await context.Response.WriteAsync("hello world!!");
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
                //pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
