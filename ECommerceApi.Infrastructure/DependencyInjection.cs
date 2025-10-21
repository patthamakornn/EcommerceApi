
using Arch.EntityFrameworkCore.UnitOfWork;
using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Infrastructure.Authentication;
using ECommerceApi.Infrastructure.Extensions;
using ECommerceApi.Infrastructure.Persistence;
using ECommerceApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceApi.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
			services.AddUnitOfWork<ApplicationDbContext>();
			services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
			services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
			services.AddJwtAuthentication(configuration);
			services.AddScoped<IPasswordService, PasswordService>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ICartRepository, CartRepository>();
			services.AddScoped<IOrderRepository, OrderRepository>();
			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

			return services;
		}
	}
}
