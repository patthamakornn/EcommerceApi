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
	public class CartServiceTests
	{
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ICartRepository> _cartRepoMock;
		private readonly Mock<IProductRepository> _productRepoMock;
		private readonly CartService _cartService;

		public CartServiceTests()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_cartRepoMock = new Mock<ICartRepository>();
			_productRepoMock = new Mock<IProductRepository>();

			_cartService = new CartService(
				_unitOfWorkMock.Object,
				_cartRepoMock.Object,
				_productRepoMock.Object
			);
		}

		[Fact]
		public async Task CreateCartAsync_WhenCartExists_ReturnsExistingCart()
		{
			// Arrange
			var userId = Guid.NewGuid();

			var existingCart = new Cart
			{
				Id = Guid.NewGuid(),
				UserId = userId
			};

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(existingCart);

			// Act
			var result = await _cartService.CreateCartAsync(userId);

			// Assert
			var dataResult = Assert.IsType<SuccessDataResult<CartResponse>>(result);
			Assert.Equal(existingCart.Id, dataResult.Data.CartId);
		}

		[Fact]
		public async Task CreateCartAsync_WhenNoCartExists_CreatesNewCart()
		{
			// Arrange
			var userId = Guid.NewGuid();
			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null!);

			_cartRepoMock.Setup(x => x.CreateAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);


			// Act
			var result = await _cartService.CreateCartAsync(userId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<CartResponse>>(result);
			Assert.NotEqual(Guid.Empty, successResult.Data.CartId);

			_cartRepoMock.Verify(x => x.GetCartByUserIdAsync(userId), Times.Once);
			_cartRepoMock.Verify(x => x.CreateAsync(It.IsAny<Cart>()), Times.Once);
		}

		[Fact]
		public async Task AddProductToCartAsync_CartNotFound_ReturnsNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null!);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ProductNotFound_ReturnsNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync((Product)null!);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_StockExceeded_ReturnsBadRequest()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var existingCartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 1
			};

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem> { existingCartItem }
			};

			var product = new Product
			{
				Id = productId,
				StockQuantity = 1,
				Price = 9999
			};

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, Guid.NewGuid(), productId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), error.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_NewProduct_AddsToCart()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem>()
			};

			var product = new Product
			{
				Id = productId,
				StockQuantity = 10,
				Price = 9999
			};

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);
			_cartRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Cart>()));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, Guid.NewGuid(), productId);

			// Assert
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), result.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ExistingProduct_IncreasesQuantity()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var cartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 1,
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow
			};

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem> { cartItem }
			};

			var product = new Product
			{
				Id = productId,
				StockQuantity = 10,
				Price = 9999
			};

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);
			_cartRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Cart>()));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, Guid.NewGuid(), productId);

			// Assert
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), result.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_WhenExceptionThrown_ReturnsErrorResult()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ThrowsAsync(new Exception("Unexpected"));

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, Guid.NewGuid(), productId);

			// Assert
			var error = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), error.StatusCode);
		}
	}
}