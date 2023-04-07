using System.Resources;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentityServer()   
                .AddInMemoryClients(IdentityServerConfig.GetClients())// 方便測試選擇in-memory
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryApiScopes(IdentityServerConfig.GetApiScopes())
                .AddDeveloperSigningCredential();// 方便開發階段於啟動時產生暫時密鑰(tempkey.jwk)

            builder.Services.AddAuthentication("access_token")
                .AddCookie("access_token", options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            var app = builder.Build();

            app.MapGet("/", () => "Hello Identity Server!");

            // 啟用IdentityServer
            app.UseIdentityServer();

            app.Run();

        }
    }
}
