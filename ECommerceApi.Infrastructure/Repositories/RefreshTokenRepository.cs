using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly ApplicationDbContext _context;

		public RefreshTokenRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<RefreshToken?> GetTokenAsync(Guid userId, string accessToken)
		{
			return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(rt =>
					rt.UserId == userId &&
					rt.AccessToken == accessToken
				);
		}
		public async Task<RefreshToken?> GetWithUserByTokenAsync(string refreshToken)
		{
			return await _context.RefreshTokens
				.Include(rt => rt.User)
				.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
		}

		public async Task AddAsync(RefreshToken token)
		{
			await _context.RefreshTokens.AddAsync(token);
		}

		public async Task DeleteOldTokensAsync(Guid userId)
		{
			var oldTokens = await _context.RefreshTokens.Where(t => t.UserId == userId).ToListAsync();

			if (oldTokens.Any())
			{
				_context.RefreshTokens.RemoveRange(oldTokens);
			}
		}
	}
}
