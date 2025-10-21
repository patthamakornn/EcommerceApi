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
	[Route("api/order")]
	public class OrderController : BaseController
	{
		private readonly IOrderService _orderService;
		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpPost("checkout")]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<OrderResponse>))]
		public async Task<IActionResult> Checkout()
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _orderService.CheckoutAsync(userId);

			if (result is null)
				return BadRequest("Cart is empty or invalid.");

			return Result(result);
		}

		[HttpGet("orders")]
		[ProducesResponseType(200, Type = typeof(SuccessDataResult<List<OrderDto>>))]
		public async Task<IActionResult> GetUserOrders()
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!Guid.TryParse(userIdString, out var userId))
				return Unauthorized();

			var result = await _orderService.GetUserOrdersAsync(userId);
			return Result(result);
		}

	}
}
