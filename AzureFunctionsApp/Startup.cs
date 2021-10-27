using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System.IO;

[assembly: FunctionsStartup(typeof(AzureFunctionsApp.Startup))]
namespace AzureFunctionsApp
{
	class Startup : FunctionsStartup
	{
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string entryAssembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string callingAssemby = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location);
            string executingAssembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            File.WriteAllText("d:\\test.txt", entryAssembly + "\r\n" + callingAssemby + "\r\n" + executingAssembly);
            builder.Services.AddLogging(loggingBuilder =>
            {
                var nLogOptions = new NLogAspNetCoreOptions
                {
                    RegisterHttpContextAccessor = true,
                    IgnoreEmptyEventId = true,
                    IncludeScopes = true,
                    ShutdownOnDispose = true
                };

                var logFactory = NLogBuilder.ConfigureNLog(Path.Combine(executingAssembly, "..", "nlog.config"));
                logFactory.AutoShutdown = false;

                var nLogConfig = logFactory.Configuration;
                loggingBuilder.AddNLog(nLogConfig, nLogOptions);
            });
        }
    }
}
