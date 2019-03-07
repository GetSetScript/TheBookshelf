using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BookShelf
{
    /// <summary>
    /// Represents the entry point of the program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry method for the application
        /// </summary>
        /// <param name="args">A collection of command line arguments used for optional configuration of the application</param>
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("Initialized Program");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program becuase of error");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }

        }

        /// <summary>
        /// Builds the Web Host that is used to run the site
        /// </summary>
        /// <param name="args">A collection of arguments used to optionally configure the Web Host</param>
        /// <returns>Returns a <see cref="IWebHostBuilder"/> configured to run the site</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}
