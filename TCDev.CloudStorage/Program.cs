// TCDev.de 2020/01/29
// TCDev.CloudStorage.Program.cs
// https://www.github.com/deejaytc/dotnet-utils

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TCDev.CloudStorage
{
   public class Program
   {
      public static void Main(string[] args)
      {
         BuildWebHost(args)
            .Run();
      }

      public static IWebHost BuildWebHost(string[] args)
      {
         var host = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .ConfigureLogging((hostingContext, logging) =>
            {
               logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
               logging.AddConsole();
               logging.AddDebug();
            })
            .UseUrls("http://0.0.0.0:5000/")
            .Build();

         host.Run();
         return host;
      }
   }
}
