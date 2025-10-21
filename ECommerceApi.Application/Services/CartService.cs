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
	public class CartService : ICartService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICartRepository _cartRepo;
		private readonly IProductRepository _productRepo;

		public CartService(IUnitOfWork unitOfWork, ICartRepository cartRepo, IProductRepository productRepo)
		{
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
			_cartRepo = cartRepo ?? throw new ArgumentNullException(nameof(cartRepo));
			_productRepo = productRepo ?? throw new ArgumentNullException(nameof(productRepo));
		}

		public async Task<ICustomResult> CreateCartAsync(Guid userId)
		{
			try
			{
				var existingCart = await _cartRepo.GetCartByUserIdAsync(userId);
				if (existingCart is not null)
				{
					var cartResponse = new CartResponse { CartId = existingCart.Id };
					Log.Information("CreateCartAsync : Found existing cart for userId {UserId}, cartId {CartId}", userId, existingCart.Id);
					return new SuccessDataResult<CartResponse>(cartResponse);
				}

				var cart = new Cart
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					CreatedDate = DateTime.UtcNow,
					UpdatedDate = DateTime.UtcNow
				};

				await _cartRepo.CreateAsync(cart);

				var response = new CartResponse { CartId = cart.Id };
				return new SuccessDataResult<CartResponse>(response);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "CreateCartAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<ICustomResult> AddProductToCartAsync(Guid userId, Guid cartId, Guid productId)
		{
			try
			{
				var cart = await _cartRepo.GetCartByUserIdAsync(userId);

				if (cart is null || cart.UserId != userId) 
				{
					Log.Information("AddProductToCartAsync : Cart not found or access denied");
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Cart not found or access denied.");
				}

				var product = await _productRepo.GetProductByIdAsync(productId);

				if (product is null)
				{
					Log.Information("AddProductToCartAsync : Product not found");
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Product not found.");
				}

				var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

				int newQuantity = existingCartItem is not null ? existingCartItem.Quantity + 1 : 1;

				if (newQuantity > product.StockQuantity)
				{
					Log.Error("AddProductToCartAsync : Cannot add more than available stock");
					return new ErrorDataResult<string>(HttpStatusCode.BadRequest, "Cannot add more than available stock.");
				}

				if (existingCartItem is not null)
				{
					existingCartItem.Quantity = newQuantity;
					existingCartItem.UpdatedDate = DateTime.UtcNow;
				}
				else
				{
					var cartItem = new CartItem
					{
						ProductId = productId,
						Quantity = 1,
						Price = product.Price,
						CreatedDate = DateTime.UtcNow,
						UpdatedDate = DateTime.UtcNow
					};
					cart.CartItems.Add(cartItem);
				}

				_cartRepo.UpdateAsync(cart);
				await _unitOfWork.SaveChangesAsync();

				return new Result(HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "AddProductToCartAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<ICustomResult> GetCartByIdForUserAsync(Guid cartId, Guid userId)
		{
			try
			{
				var cart = await _cartRepo.GetCartByIdAndUserIdAsync(cartId, userId);

				if (cart is null)
				{
					Log.Information("GetCartByIdForUserAsync : Cart not found");
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Cart not found");
				}

				var cartDto = new CartDto
				{
					CartId = cart.Id,
					Items = cart.CartItems.Select(ci => new CartItemDto
					{
						ProductId = ci.ProductId,
						ProductName = ci.Product.ProductName,
						Quantity = ci.Quantity,
						Price = ci.Price
					}).ToList(),
					TotalAmount = cart.CartItems.Sum(i => i.Price * i.Quantity)
				};

				return new SuccessDataResult<CartDto>(cartDto);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "GetCartByIdForUserAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

		public async Task<ICustomResult> RemoveProductFromCartAsync(Guid userId, Guid cartId, Guid productId)
		{
			try
			{
				var cart = await _cartRepo.GetByUserIdAndCartIdAsync(userId, cartId);

				if (cart is null)
				{
					Log.Information("RemoveProductFromCartAsync : Cart not found");
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Cart not found.");
				}

				var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

				if (cartItem is null)
				{
					Log.Information("RemoveProductFromCartAsync : Product not found in cart");
					return new ErrorDataResult<string>(HttpStatusCode.NotFound, "Product not found in cart.");
				}

				if (cartItem.Quantity > 1)
				{
					cartItem.Quantity -= 1;
					cartItem.UpdatedDate = DateTime.UtcNow;
				}
				else
				{
					cart.CartItems.Remove(cartItem);
					cart.UpdatedDate = DateTime.UtcNow;
				}

				_cartRepo.UpdateAsync(cart);
				await _unitOfWork.SaveChangesAsync();

				return new Result(HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RemoveProductFromCartAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

	}
}
