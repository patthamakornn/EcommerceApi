using ECommerceApi.Application.Models.Common;
using System.Net;
using System.Text.Json;

namespace ECommerceApi.API.Middleware
{
	public class ApiResponseMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ApiResponseMiddleware> _logger;

		public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);

				if (!context.Response.HasStarted)
				{
					context.Response.ContentType = "application/json";
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

					var response = new ErrorDataResult<object>(HttpStatusCode.InternalServerError, ex.Message);

					await context.Response.WriteAsync(JsonSerializer.Serialize(response));
				}

			}
		}
	}
}
