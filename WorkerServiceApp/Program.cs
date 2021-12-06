using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using System.IO;
using Microsoft.Extensions.Hosting;
using NLog;

namespace WorkerServiceApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseWindowsService()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddLogging(loggingBuilder =>
					{
						var nLogOptions = new NLogAspNetCoreOptions
						{
							RegisterHttpContextAccessor = true,
							IgnoreEmptyEventId = true,
							IncludeScopes = true,
							ShutdownOnDispose = true
						};

						string executingAssembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
						var logFactory = NLogBuilder.ConfigureNLog(Path.Combine(executingAssembly, "nlog.config"));
						logFactory.AutoShutdown = false;
						var nLogConfig = logFactory.Configuration;
						GlobalDiagnosticsContext.Set("AppName", "WorkerServiceApp");
						loggingBuilder.AddNLog(nLogConfig, nLogOptions);
					});
					services.AddHostedService<ExtractDocument>();
				});
	}
}
