using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SunnysideStablesAPI.Data;

namespace SunnysideStablesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // CreateHostBuilder(args).Build().Run();

            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AppDbContext>();
                DBInitializer.SeedDataBase(services);
            }

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
         }
}
