using Arch.EntityFrameworkCore.UnitOfWork;
using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using Moq;
using System.Net;

namespace ECommerceApi.UnitTests.Services
{
	public class OrderServiceTest
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ICartRepository> _cartRepoMock;
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly OrderService _orderService;

		public OrderServiceTest()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_cartRepoMock = new Mock<ICartRepository>();
			_orderRepoMock = new Mock<IOrderRepository>();

			_orderService = new OrderService(
				_unitOfWorkMock.Object,
				_cartRepoMock.Object,
				_orderRepoMock.Object
			);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsNotFound_WhenCartIsNull()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ReturnsAsync((Cart?)null);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsNotFound_WhenCartItemsEmpty()
		{
			// Arrange
			var userId = Guid.NewGuid();

			var cart = new Cart { CartItems = new List<CartItem>() };

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsBadRequest_WhenProductIsNull()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cart = new Cart
			{
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						ProductId = Guid.NewGuid(),
						Quantity = 1,
						Price = 100,
						Product = null!
					}
				}
			};

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsBadRequest_WhenProductStockIsNotEnough()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var cart = new Cart
			{
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						ProductId = productId,
						Quantity = 5,
						Price = 100,
						Product = new Product
						{
							Id = productId,
							ProductName = "Test",
							StockQuantity = 3
						}
					}
				}
			};

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsSuccess_WhenCheckoutIsSuccessful()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var product = new Product
			{
				Id = productId,
				ProductName = "Test Product",
				StockQuantity = 10,
				Price = 100
			};

			var cartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 2,
				Price = 100,
				Product = product
			};

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem> { cartItem }
			};

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ReturnsAsync(cart);
			_orderRepoMock.Setup(x => x.Create(It.IsAny<Order>()));
			_cartRepoMock.Setup(x => x.Update(It.IsAny<Cart>()));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<OrderResponse>>(result);
			Assert.NotEqual(Guid.Empty, successResult.Data?.OrderId);
		}

		[Fact]
		public async Task CheckoutAsync_ReturnsInternalServerError_WhenException()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId)).ThrowsAsync(new Exception("error"));

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task GetUserOrdersAsync_ReturnsSuccessDataResult_WhenOrdersExist()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();
			var orderId = Guid.NewGuid();

			var orders = new List<Order>
			{
				new Order
				{
					Id = orderId,
					UserId = userId,
					OrderDate = DateTime.UtcNow,
					TotalAmount = 100,
					OrderItems = new List<OrderItem>
					{
						new OrderItem
						{
							ProductId = productId,
							Quantity = 2,
							Price = 50,
							Product = new Product
							{
								Id = productId,
								ProductName = "Test Product"
							}
						}
					}
				}
			};

			_orderRepoMock.Setup(x => x.GetOrdersByUserIdAsync(userId)).ReturnsAsync(orders);

			// Act
			var result = await _orderService.GetUserOrdersAsync(userId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<List<OrderDto>>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
		}

		[Fact]
		public async Task GetUserOrdersAsync_ReturnsInternalServerError_WhenException()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_orderRepoMock.Setup(x => x.GetOrdersByUserIdAsync(userId)).ThrowsAsync(new Exception("error"));

			// Act
			var result = await _orderService.GetUserOrdersAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}
	}
}
