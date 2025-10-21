
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Authentication
{
	public interface IJwtTokenGenerator
	{
		string GenerateToken(User user);
		DateTime GetExpiration(string token);
		bool IsTokenExpired(string token);
	}

}
