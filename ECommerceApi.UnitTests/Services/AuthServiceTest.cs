using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using Moq;
using System.Net;

namespace ECommerceApi.UnitTests.Services
{
	public class AuthServiceTest
	{
		private readonly Mock<IUserRepository> _userRepoMock;
		private readonly Mock<IPasswordService> _passwordServiceMock;
		private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
		private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
		private readonly AuthService _authService;

		public AuthServiceTest()
		{
			_userRepoMock = new Mock<IUserRepository>();
			_passwordServiceMock = new Mock<IPasswordService>();
			_jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
			_refreshTokenServiceMock = new Mock<IRefreshTokenService>();

			_authService = new AuthService(
				_userRepoMock.Object,
				_passwordServiceMock.Object,
				_jwtTokenGeneratorMock.Object,
				_refreshTokenServiceMock.Object
			);
		}

		[Fact]
		public async Task RegisterAsync_ReturnsBadRequest_WhenEmailAlreadyExists()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Email = "test@example.com",
				Password = "password",
				FirstName = "Test",
				LastName = "User"
			};

			_userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(new User());

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task RegisterAsync_ReturnsCreateDataResult_WhenUserIsCreatedSuccessful()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Email = "test@example.com",
				Password = "password",
				FirstName = "Test",
				LastName = "User"
			};

			_userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync((User?)null);
			_passwordServiceMock.Setup(p => p.HashPassword(request.Password)).Returns("hashedPassword");

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			var createResult = Assert.IsType<CreateDataResult>(result); 
			Assert.Equal(HttpStatusCode.Created.GetHashCode(), createResult.StatusCode);
		}

		[Fact]
		public async Task RegisterAsync_ReturnsInternalServerError_WhenSaveFail()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Email = "test@example.com",
				Password = "password",
				FirstName = "Test",
				LastName = "User"
			};

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email)).ReturnsAsync((User)null!);
			_passwordServiceMock.Setup(x => x.HashPassword(request.Password)).Returns("hashed");
			_userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ThrowsAsync(new Exception("errorResult"));

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_ReturnsUnauthorized_WhenUserIsNull()
		{
			// Arrange
			var request = new LoginRequest 
			{ 
				Email = "notfound@example.com",
				Password = "pass123" 
			};

			_userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync((User?)null);

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.Unauthorized.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_ReturnsUnauthorized_WhenPasswordIsInvalid()
		{
			// Arrange
			var request = new LoginRequest
			{
				Email = "user@example.com",
				Password = "wrongpass" 
			};

			var user = new User { Email = request.Email, PasswordHash = "correctHashedPassword" };
			_userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
			_passwordServiceMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash)).Returns(false);

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.Unauthorized.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_ReturnsSuccessDataResult_WhenLoginIsSuccessful()
		{
			// Arrange
			var request = new LoginRequest 
			{ 
				Email = "user@example.com",
				Password = "correctpass"
			};

			var user = new User 
			{ 
				Email = request.Email, 
				PasswordHash = "hashedPass" 
			};

			var tokenResponse = new RefreshToken
			{
				AccessToken = "access-token",
				Token = "refresh-token",
			};

			var expiresAt = DateTime.Now.AddHours(1);

			_userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
			_passwordServiceMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash)).Returns(true);
			_refreshTokenServiceMock.Setup(r => r.GenerateTokenAsync(user)).ReturnsAsync(tokenResponse);
			_jwtTokenGeneratorMock.Setup(j => j.GetExpiration(tokenResponse.AccessToken)).Returns(expiresAt.ToUniversalTime());

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<LoginResponse>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
			Assert.Equal(tokenResponse.AccessToken, successResult.Data.AccessToken);
			Assert.Equal(tokenResponse.Token, successResult.Data.RefreshToken);
		}

		[Fact]
		public async Task LoginAsync_ReturnsInternalServerError_WhenException()
		{
			// Arrange
			var request = new LoginRequest
			{
				Email = "user@example.com",
				Password = "correctpass"
			};

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email)).ThrowsAsync(new Exception("Something went wrong"));

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}

	}
}