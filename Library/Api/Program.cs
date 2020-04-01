using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Rollbar;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var s17437logger = NLog.Web.NLogBuilder.ConfigureNLog("s17437nlog.config").GetCurrentClassLogger();

            try
            {
                RollbarLocator.RollbarInstance.Configure(new RollbarConfig("6b6bf76c663e4a6691f519ea33b89d6d"));
                RollbarLocator.RollbarInstance.Info("Rollbar is configured properly.");

                s17437logger.Debug("aplikacja działa");
                RollbarLocator.RollbarInstance.Info("App has started.");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (System.Exception exception)
            {
                s17437logger.Error(exception, "Program spadł z rowerka");
                RollbarLocator.RollbarInstance.Error("App encountered an exception.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddUserSecrets(appAssembly, optional: true)
                        .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                }).UseNLog();

    }
}
