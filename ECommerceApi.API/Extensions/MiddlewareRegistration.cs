using ECommerceApi.API.Middleware;

namespace ECommerceApi.API.Extensions
{
	public static class MiddlewareRegistration
	{
		public static void UseMiddlewareRegistration(this IApplicationBuilder builder)
		{
			builder.UseMiddleware<ApiResponseMiddleware>();
		}
	}
}
