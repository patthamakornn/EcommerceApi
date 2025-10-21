using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IRefreshTokenRepository
	{
		Task<RefreshToken?> GetTokenAsync(Guid userId, string accessToken);
		Task<RefreshToken?> GetWithUserByTokenAsync(string token);
		Task AddAsync(RefreshToken token);
		Task DeleteOldTokensAsync(Guid userId);
	}
}
