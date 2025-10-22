using ECommerceApi.Application.Interfaces.Authentication;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Domain.Entities;
using Serilog;
using System.Net;

namespace ECommerceApi.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepo;
		private readonly IPasswordService _passwordService;
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		private readonly IRefreshTokenService _refreshTokenService;

		public AuthService(IUserRepository userRepo, IPasswordService passwordService, IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenService refreshTokenService)
		{
			_userRepo = userRepo;
			_passwordService = passwordService;
			_jwtTokenGenerator = jwtTokenGenerator;
			_refreshTokenService = refreshTokenService;
		}

		public async Task<ICustomResult> RegisterAsync(RegisterRequest request)
		{
			try
			{
				var existingUser = await _userRepo.GetUserByEmailAsync(request.Email);
				if (existingUser is not null)
				{
					Log.Information("RegisterAsync failed: Email already exists - {Email}", request.Email);
					return new ErrorDataResult<string>(HttpStatusCode.BadRequest, "Email already exists");
				}

				var hashedPassword = _passwordService.HashPassword(request.Password);

				var user = new User
				{
					Email = request.Email,
					PasswordHash = hashedPassword,
					FirstName = request.FirstName,
					LastName = request.LastName,
				};

				await _userRepo.CreateAsync(user);

				return new CreateDataResult();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RegisterAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<ICustomResult> LoginAsync(LoginRequest request)
		{
			try
			{
				var user = await _userRepo.GetUserByEmailAsync(request.Email);

				if (user is null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
				{
					Log.Information("LoginAsync failed: Invalid credentials for email: {Email}", request.Email);
					return new ErrorDataResult<string>(HttpStatusCode.Unauthorized, "Invalid username or password");
				}

				var token = await _refreshTokenService.GenerateTokenAsync(user);
				var expiresAt = _jwtTokenGenerator.GetExpiration(token.AccessToken).ToLocalTime();

				var response = new LoginResponse
				{
					AccessToken = token.AccessToken,
					RefreshToken = token.Token,
					ExpiresAt = expiresAt
				};

				return new SuccessDataResult<LoginResponse>(response);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "LoginAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}
