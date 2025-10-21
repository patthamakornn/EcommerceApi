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
		public async Task CheckoutAsync_CartNotFound_ReturnsNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ReturnsAsync((Cart)null!);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_EmptyCart_ReturnsNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cart = new Cart { CartItems = new List<CartItem>() };

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_ProductIsNull_ReturnsBadRequest()
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

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_InsufficientStock_ReturnsBadRequest()
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

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ReturnsAsync(cart);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task CheckoutAsync_Success_ReturnsOrderId()
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

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ReturnsAsync(cart);

			_orderRepoMock.Setup(x => x.CreateAsync(It.IsAny<Order>()));

			_cartRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Cart>()));

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>()))
				.ReturnsAsync(1);

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var success = Assert.IsType<SuccessDataResult<OrderResponse>>(result);
			Assert.NotEqual(Guid.Empty, success.Data.OrderId);
		}

		[Fact]
		public async Task CheckoutAsync_ExceptionThrown_ReturnsError()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartWithItemsAndProductsAsync(userId))
				.ThrowsAsync(new Exception("failed"));

			// Act
			var result = await _orderService.CheckoutAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task GetUserOrdersAsync_WhenOrdersExist_ReturnsSuccess()
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
			Assert.Single(successResult.Data);

			var dto = successResult.Data.First();
			Assert.Equal(orderId, dto.OrderId);
			Assert.Equal(100, dto.TotalAmount);
			Assert.Single(dto.Items);

			var item = dto.Items.First();
			Assert.Equal(productId, item.ProductId);
			Assert.Equal("Test Product", item.ProductName);
			Assert.Equal(2, item.Quantity);
			Assert.Equal(50, item.Price);
		}

		[Fact]
		public async Task GetUserOrdersAsync_WhenExceptionThrown_ReturnsError()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_orderRepoMock.Setup(x => x.GetOrdersByUserIdAsync(userId))
				.ThrowsAsync(new Exception("failed"));

			// Act
			var result = await _orderService.GetUserOrdersAsync(userId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), error.StatusCode);
		}
	}
}
