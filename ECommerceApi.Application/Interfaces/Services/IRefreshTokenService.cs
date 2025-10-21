using ECommerceApi.Application.Models.Common;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IRefreshTokenService
	{
		Task<bool> ValidateAccessTokenAsync(Guid userId, string accessToken);
		Task<ICustomResult> RefreshTokensAsync(string refreshToken);
		Task<RefreshToken> GenerateTokenAsync(User user);
	}
}
