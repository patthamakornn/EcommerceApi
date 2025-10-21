using Serilog;
using Serilog.Events;

namespace ECommerceApi.API.Extensions
{
	public static class LoggingServiceExtensions
	{
		public static IHostBuilder UseFileLogging(this IHostBuilder hostBuilder, string fileName = "application.log")
		{
			return hostBuilder.UseSerilog((context, services, loggerConfig) =>
			{
				loggerConfig
					.MinimumLevel.Information()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)  // ลด log ของ Microsoft
					.Enrich.FromLogContext()
					.WriteTo.File(
						path: $"logs/{fileName}",
						rollingInterval: RollingInterval.Day,
						outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
						fileSizeLimitBytes: 10 * 1024 * 1024,  // 10 MB
						rollOnFileSizeLimit: true,
						shared: true
					);
			});
		}
	}
}
