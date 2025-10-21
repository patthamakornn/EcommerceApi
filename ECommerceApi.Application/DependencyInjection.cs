using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace ECommerceApi.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IRefreshTokenService, RefreshTokenService>();
			services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());
			return services;
		}
	}
}
