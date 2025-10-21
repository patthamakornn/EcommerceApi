using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IAuthService
	{
		Task<ICustomResult> RegisterAsync(RegisterRequest request);
		Task<ICustomResult> LoginAsync(LoginRequest request);
	}
}
