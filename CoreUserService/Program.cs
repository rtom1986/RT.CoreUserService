using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CoreUserService
{
    public class Program
    {
        /// <summary>
        /// Application entry point
        /// </summary>
        /// <param name="args">optional arguments</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
