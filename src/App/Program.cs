using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: InternalsVisibleTo("App.Tests")]

namespace EnChanger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog(LoggerConfiguration);

        private static void LoggerConfiguration(HostBuilderContext host,
            LoggerConfiguration configuration)
        {
            configuration.ReadFrom.Configuration(host.Configuration)
                .WriteTo.Console()
                .WriteTo.Debug();
        }
    }
}
