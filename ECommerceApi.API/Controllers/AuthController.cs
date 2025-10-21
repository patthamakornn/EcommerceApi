using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers
{
	[ApiController]
	[Route("api/auth")]
	[Authorize]
	public class AuthController : BaseController
	{
		private readonly IAuthService _authService;
		private readonly IRefreshTokenService _refreshTokenService;
		public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
		{
			_authService = authService;
			_refreshTokenService = refreshTokenService;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		[ProducesResponseType(200, Type = typeof(Result))]
		public async Task<IActionResult> Register(RegisterRequest request)
		{
			var result = await _authService.RegisterAsync(request);
			return Result(result);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<LoginResponse>))]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var result = await _authService.LoginAsync(request);
			return Result(result);
		}

		[HttpPost("refresh")]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<RefreshTokenResponse>))]
		public async Task<IActionResult> Refresh(RefreshTokenRequest request)
		{
			var result = await _refreshTokenService.RefreshTokensAsync(request.RefreshToken);
			return Result(result);
		}
	}
}
