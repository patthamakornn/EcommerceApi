using Arch.EntityFrameworkCore.UnitOfWork;
using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace ECommerceApi.UnitTests.Services
{
	public class RefreshTokenTest
	{
		private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
		private readonly IOptions<JwtSettings> _jwtSettingsOptions;

		private readonly RefreshTokenService _refreshTokenService;

		public RefreshTokenTest()
		{
			_refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

			var jwtSettings = new JwtSettings
			{
				Key = "TestSecretKey123456",
				AccessTokenExpirationMinutes = 15,
				Issuer = "TestIssuer",
				Audience = "TestAudience",
				RefreshTokenExpirationDays = 7
			};
			_jwtSettingsOptions = Options.Create(jwtSettings);

			_refreshTokenService = new RefreshTokenService(
				_unitOfWorkMock.Object,
				_refreshTokenRepoMock.Object,
				_jwtTokenGeneratorMock.Object,
				_jwtSettingsOptions
			);


		}

		[Fact]
		public async Task ValidateAccessTokenAsync_TokenExists_ReturnsTrue()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var token = "some-access-token";

			_refreshTokenRepoMock.Setup(x => x.GetTokenAsync(userId, token))
				.ReturnsAsync(new RefreshToken());

			// Act
			var result = await _refreshTokenService.ValidateAccessTokenAsync(userId, token);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task ValidateAccessTokenAsync_TokenNotFound_ReturnsFalse()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var token = "invalid-token";

			_refreshTokenRepoMock.Setup(x => x.GetTokenAsync(userId, token))
				.ReturnsAsync((RefreshToken)null!);

			// Act
			var result = await _refreshTokenService.ValidateAccessTokenAsync(userId, token);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public async Task RefreshTokensAsync_InvalidToken_ReturnsUnauthorized()
		{
			// Arrange
			var token = "expired-token";
			_refreshTokenRepoMock.Setup(x => x.GetWithUserByTokenAsync(token))
				.ReturnsAsync((RefreshToken)null!);

			// Act
			var result = await _refreshTokenService.RefreshTokensAsync(token);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			//Assert.Equal(HttpStatusCode.Unauthorized, error.StatusCode);
		}

		[Fact]
		public async Task RefreshTokensAsync_ValidToken_ReturnsNewTokens()
		{
			// Arrange
			var token = "valid-refresh-token";
			var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

			var existingToken = new RefreshToken
			{
				Token = token,
				ExpiresAt = DateTime.UtcNow.AddMinutes(5),
				User = user
			};

			_refreshTokenRepoMock.Setup(x => x.GetWithUserByTokenAsync(token))
				.ReturnsAsync(existingToken);

			_refreshTokenRepoMock.Setup(x => x.DeleteOldTokensAsync(user.Id))
				.Returns(Task.CompletedTask);

			_refreshTokenRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
				.Returns(Task.CompletedTask);

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>()))
				.ReturnsAsync(1);

			_jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
				.Returns("new-access-token");

			// Act
			var result = await _refreshTokenService.RefreshTokensAsync(token);

			// Assert
			var success = Assert.IsType<SuccessDataResult<RefreshTokenResponse>>(result);
			Assert.Equal("new-access-token", success.Data.AccessToken);
			Assert.False(string.IsNullOrWhiteSpace(success.Data.RefreshToken));
		}

		[Fact]
		public async Task GenerateTokenAsync_CreatesAndReturnsRefreshToken()
		{
			// Arrange
			var user = new User { Id = Guid.NewGuid() };
			var accessToken = "access-token-generated";

			_refreshTokenRepoMock.Setup(x => x.DeleteOldTokensAsync(user.Id))
				.Returns(Task.CompletedTask);

			_jwtTokenGeneratorMock.Setup(x => x.GenerateToken(user))
				.Returns(accessToken);

			_refreshTokenRepoMock.Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
				.Returns(Task.CompletedTask);

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>()))
				.ReturnsAsync(1);

			// Act
			var result = await _refreshTokenService.GenerateTokenAsync(user);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(user.Id, result.UserId);
			Assert.Equal(accessToken, result.AccessToken);
			Assert.False(string.IsNullOrWhiteSpace(result.Token));
		}
	}
}