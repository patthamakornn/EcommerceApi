using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApi.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/cart")]
	public class CartController : BaseController
	{
		private readonly ICartService _cartService;

		public CartController(ICartService cartService)
		{
			_cartService = cartService;
		}

		[HttpPost]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<CartResponse>))]
		public async Task<IActionResult> CreateCart()
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _cartService.CreateCartAsync(userId);
			return Result(result);
		}

		[HttpPost("{cartId}/products/{productId}")]
		[ProducesResponseType(200, Type = typeof(Result))]
		public async Task<IActionResult> AddProductToCart(Guid cartId, Guid productId)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);
			return Result(result);
		}

		[HttpGet("{cartId}")]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<CartDto>))]
		public async Task<IActionResult> GetCartById(Guid cartId)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _cartService.GetCartByIdForUserAsync(cartId, userId);
			return Result(result);
		}

		[HttpDelete("{cartId}/products/{productId}")]
		[ProducesResponseType(200, Type = typeof(Result))]
		public async Task<IActionResult> RemoveProductFromCart(Guid cartId, Guid productId)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _cartService.RemoveProductFromCartAsync(userId, cartId, productId);
			return Result(result);
		}

	}
}
