using System.Resources;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityServer()
                // 方便測試選擇in-memory
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
                // 方便開發階段於啟動時產生暫時密鑰(tempkey.jwk)
                .AddDeveloperSigningCredential();

            builder.Services.AddAuthentication("MyCookie")
                .AddCookie("MyCookie", options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });

            var app = builder.Build();

            app.MapGet("/", () => "Hello Identity Server!");

            // 啟用IdentityServer
            app.UseIdentityServer();

            app.Run();

        }
    }
}
