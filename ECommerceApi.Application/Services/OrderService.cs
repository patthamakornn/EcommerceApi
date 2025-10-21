using Arch.EntityFrameworkCore.UnitOfWork;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Domain.Entities;
using Serilog;
using System.Net;

namespace ECommerceApi.Application.Services
{
	public class OrderService : IOrderService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICartRepository _cartRepo;
		private readonly IOrderRepository _orderRepo;
		public OrderService(IUnitOfWork unitOfWork, ICartRepository cartRepo, IOrderRepository orderRepo)
		{
			_unitOfWork = unitOfWork;
			_cartRepo = cartRepo;
			_orderRepo = orderRepo;
		}

		public async Task<ICustomResult> CheckoutAsync(Guid userId)
		{
			try
			{
				var cart = await _cartRepo.GetCartWithItemsAndProductsAsync(userId);

				if (cart is null || cart.CartItems.Count == 0)
				{
					Log.Information("CheckoutAsync : Cart not found or item empty for userId {UserId}", userId);
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Cart not found or item empty.");
				}

				var order = new Order
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					OrderDate = DateTime.UtcNow,
					TotalAmount = cart.CartItems.Sum(ci => ci.Price * ci.Quantity)
				};

				foreach (var cartItem in cart.CartItems)
				{
					var product = cartItem.Product;

					if (product is null)
					{
						Log.Information("CheckoutAsync : Product {ProductId} not found", cartItem.ProductId);
						return new ErrorDataResult<string>(HttpStatusCode.BadRequest, $"Product : {cartItem.ProductId} not found.");
					}

					if (product.StockQuantity < cartItem.Quantity)
					{
						Log.Information("CheckoutAsync : Not enough stock for product {ProductName} ", product.ProductName);
						return new ErrorDataResult<string>(HttpStatusCode.BadRequest, $"Not enough stock for product {product.ProductName}.");
					}

					product.StockQuantity -= cartItem.Quantity;

					order.OrderItems.Add(new OrderItem
					{
						Id = Guid.NewGuid(),
						OrderId = order.Id,
						ProductId = cartItem.ProductId,
						Quantity = cartItem.Quantity,
						Price = cartItem.Price
					});
				}

				_orderRepo.CreateAsync(order);
				cart.CartItems.Clear();
				cart.UpdatedDate = DateTime.UtcNow;

				_cartRepo.UpdateAsync(cart);
				await _unitOfWork.SaveChangesAsync();

				var response = new OrderResponse { OrderId = order.Id };
				return new SuccessDataResult<OrderResponse>(response);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "CheckoutAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}
		public async Task<ICustomResult> GetUserOrdersAsync(Guid userId)
		{
			try
			{
				var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);

				var orderDtos = orders.OrderByDescending(o => o.OrderDate)
					.Select(o => new OrderDto
					{
						OrderId = o.Id,
						OrderDate = o.OrderDate,
						TotalAmount = o.TotalAmount,
						Items = o.OrderItems.Select(oi => new OrderItemDto
						{
							ProductId = oi.ProductId,
							ProductName = oi.Product.ProductName,
							Quantity = oi.Quantity,
							Price = oi.Price
						}).ToList()
					}).ToList();

				return new SuccessDataResult<List<OrderDto>>(orderDtos);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "GetUserOrdersAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

	}
}
