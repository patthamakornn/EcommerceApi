using Arch.EntityFrameworkCore.UnitOfWork;
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
		public async Task RegisterAsync_WhenEmailExists_ReturnsBadRequest()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Email = "test@xyzz.com",
				Password = "password",
				FirstName = "Test",
				LastName = "User"
			};

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email))
				.ReturnsAsync(new User());

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task RegisterAsync_WhenSuccess_ReturnsSuccessMessage()
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
			_userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), result.StatusCode);
		}

		[Fact]
		public async Task RegisterAsync_WhenSaveFails_ReturnsInternalServerError()
		{
			// Arrange
			var request = new RegisterRequest
			{
				Email = "test@example.com",
				Password = "password",
				FirstName = "Test",
				LastName = "User"
			};

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email))
				.ReturnsAsync((User)null!);

			_passwordServiceMock.Setup(x => x.HashPassword(request.Password))
				.Returns("hashed");

			_userRepoMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
				.ThrowsAsync(new Exception("error"));

			// Act
			var result = await _authService.RegisterAsync(request);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_InvalidEmail_ReturnsUnauthorized()
		{
			// Arrange
			var request = new LoginRequest { Email = "abcde@example.com", Password = "1234" };

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email))
				.ReturnsAsync((User)null!);

			// Act

			var result = await _authService.LoginAsync(request);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.Unauthorized.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_InvalidPassword_ReturnsUnauthorized()
		{
			// Arrange
			var user = new User { Email = "abcde@example.com", PasswordHash = "hashed123" };
			var request = new LoginRequest { Email = "abcde@example.com", Password = "wrong" };

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email))
				.ReturnsAsync(user);

			_passwordServiceMock.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
				.Returns(false);

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.Unauthorized.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task LoginAsync_Success_ReturnsTokens()
		{
			// Arrange
			var user = new User { Email = "abcde@example.com", PasswordHash = "hashed123" };
			var request = new LoginRequest { Email = "abcde@example.com", Password = "correct" };

			var tokenResponse = new RefreshToken
			{
				AccessToken = "access-token",
				Token = "refresh-token",
			};

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
			_passwordServiceMock.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash)).Returns(true);
			_refreshTokenServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync(tokenResponse);
			_jwtTokenGeneratorMock.Setup(x => x.GetExpiration("access-token")).Returns(DateTime.UtcNow.AddMinutes(15));

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var success = Assert.IsType<SuccessDataResult<LoginResponse>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), success.StatusCode);
			Assert.Equal("access-token", success.Data.AccessToken);
			Assert.Equal("refresh-token", success.Data.RefreshToken);
		}

		[Fact]
		public async Task LoginAsync_Exception_ReturnsInternalServerError()
		{
			// Arrange
			var request = new LoginRequest { Email = "abcde@example.com", Password = "1234" };

			_userRepoMock.Setup(x => x.GetUserByEmailAsync(request.Email)).ThrowsAsync(new Exception("Something went wrong"));

			// Act
			var result = await _authService.LoginAsync(request);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), error.StatusCode);
		}

	}
}