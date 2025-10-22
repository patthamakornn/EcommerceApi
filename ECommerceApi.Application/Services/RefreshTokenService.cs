using Arch.EntityFrameworkCore.UnitOfWork;
using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;

namespace ECommerceApi.Application.Services
{
	public class RefreshTokenService : IRefreshTokenService
	{
		private readonly IRefreshTokenRepository _refreshTokenRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		private readonly JwtSettings _jwtSettings;

		public RefreshTokenService(IUnitOfWork unitOfWork, IRefreshTokenRepository refreshTokenRepo, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtSettings> jwtSettings)
		{
			_unitOfWork = unitOfWork;
			_refreshTokenRepo = refreshTokenRepo;
			_jwtTokenGenerator = jwtTokenGenerator;
			_jwtSettings = jwtSettings.Value;
		}

		public async Task<bool> ValidateAccessTokenAsync(Guid userId, string accessToken)
		{
			var token = await _refreshTokenRepo.GetTokenAsync(userId, accessToken);

			if (token is null)
			{
				Log.Information("ValidateAccessTokenAsync : Access token validation failed for userId {UserId}", userId);
				return false;

			}
			return true;
		}

		public async Task<ICustomResult> RefreshTokensAsync(string refreshToken)
		{
			try
			{
				var token = await _refreshTokenRepo.GetWithUserByTokenAsync(refreshToken);

				if (token is null)
				{
					Log.Information("RefreshTokensAsync : Refresh token not found");
					return new ErrorDataResult<string>(HttpStatusCode.Unauthorized, "Invalid token");
				}

				if (token.ExpiresAt < DateTime.UtcNow)
				{
					Log.Information("RefreshTokensAsync : Refresh token expired for userId {UserId}", token.UserId);
					return new ErrorDataResult<string>(HttpStatusCode.Unauthorized, "Expired refresh token");
				}

				var newToken = await GenerateTokenAsync(token.User);

				var response = new RefreshTokenResponse
				{
					AccessToken = newToken.AccessToken,
					RefreshToken = newToken.Token,
				};

				return new SuccessDataResult<RefreshTokenResponse>(response);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RefreshTokensAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<RefreshToken> GenerateTokenAsync(User user)
		{
			try
			{
				await _refreshTokenRepo.DeleteOldTokensAsync(user.Id);

				var accessToken = _jwtTokenGenerator.GenerateToken(user);

				var refreshToken = new RefreshToken
				{
					UserId = user.Id,
					Token = _jwtTokenGenerator.GenerateRefreshToken(),
					AccessToken = accessToken,
					ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
				};

				await _refreshTokenRepo.AddAsync(refreshToken);
				await _unitOfWork.SaveChangesAsync();

				return refreshToken;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "GenerateTokenAsync occurs an error");
				throw;
			}
		}
	}
}
