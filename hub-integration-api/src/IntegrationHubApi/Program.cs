using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IntegrationHubApi
{
    /// <summary>
    /// Entrada do programa
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Entrada do programa
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Criação do Host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
