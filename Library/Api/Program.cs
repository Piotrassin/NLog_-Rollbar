using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var s17437logger = NLog.Web.NLogBuilder.ConfigureNLog("s17437nlog.config").GetCurrentClassLogger();

            try
            {


                s17437logger.Debug("app initialized");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (System.Exception exception)
            {
                s17437logger.Error(exception, "Program spadł z rowerka");
                throw;
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
                .UseStartup<Startup>();
    }
}
