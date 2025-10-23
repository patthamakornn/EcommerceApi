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
		public async Task CreateCartAsync_ReturnsExistingCart_WhenCartAlreadyExists()
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
			var successResult = Assert.IsType<CreateDataResult<CartResponse>>(result);
			Assert.Equal(HttpStatusCode.Created.GetHashCode(), successResult.StatusCode);
			Assert.Equal(existingCart.Id, successResult.Data.CartId);
		}

		[Fact]
		public async Task CreateCartAsync_ReturnsNewCart_WhenNoExistingCart()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByUserIdAsync(userId)).ReturnsAsync((Cart)null!);
			_cartRepoMock.Setup(x => x.CreateAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);


			// Act
			var result = await _cartService.CreateCartAsync(userId);

			// Assert
			var successResult = Assert.IsType<CreateDataResult<CartResponse>>(result);
			Assert.NotEqual(Guid.Empty, successResult.Data.CartId);
			Assert.Equal(HttpStatusCode.Created.GetHashCode(), successResult.StatusCode);
			_cartRepoMock.Verify(x => x.GetCartByUserIdAsync(userId), Times.Once);
			_cartRepoMock.Verify(x => x.CreateAsync(It.IsAny<Cart>()), Times.Once);
		}

		[Fact]
		public async Task CreateCartAsync_ReturnsInternalServerError_WhenExceptionThrown()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(c => c.GetCartByUserIdAsync(userId)).ThrowsAsync(new Exception("error"));

			// Act
			var result = await _cartService.CreateCartAsync(userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsNotFound_WhenCartNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync((Cart?)null);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsNotFound_WhenProductNotFound()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem>()
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync((Product?)null);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsBadRequest_WhenStockIsNotEnough()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			var existingCartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 1
			};

			var cart = new Cart
			{
				UserId = userId,
				CartItems = new List<CartItem> 
				{ 
					existingCartItem 
				}
			};

			var product = new Product
			{
				Id = productId,
				StockQuantity = 1,
				Price = 9999
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.BadRequest.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsSuccess_WhenProductAddedNew()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
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

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);
			_cartRepoMock.Setup(x => x.Update(It.IsAny<Cart>()));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsSuccess_WhenProductQuantityIncreased()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
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
				CartItems = new List<CartItem> 
				{ 
					cartItem 
				}
			};

			var product = new Product
			{
				Id = productId,
				StockQuantity = 10,
				Price = 9999
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_productRepoMock.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);
			_cartRepoMock.Setup(x => x.Update(It.IsAny<Cart>()));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
			Assert.Equal(2, cartItem.Quantity);
		}

		[Fact]
		public async Task AddProductToCartAsync_ReturnsInternalServerError_WhenException()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ThrowsAsync(new Exception("error"));

			// Act
			var result = await _cartService.AddProductToCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task GetCartByIdForUserAsync_ReturnsNotFound_WhenCartIsNull()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync((Cart?)null);

			// Act
			var result = await _cartService.GetCartByIdForUserAsync(cartId, userId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task GetCartByIdForUserAsync_ReturnsSuccess_WhenCartExists()
		{
			// Arrange
			var cartId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			var cart = new Cart
			{
				Id = cartId,
				CartItems = new List<CartItem>
				{
					new CartItem
					{
						ProductId = Guid.NewGuid(),
						Product = new Product { ProductName = "Test Product" },
						Quantity = 2,
						Price = 10m
					}
				}
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);

			// Act
			var result = await _cartService.GetCartByIdForUserAsync(cartId, userId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<CartDto>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
			Assert.Equal(cartId, successResult.Data.CartId);
		}

		[Fact]
		public async Task RemoveProductFromCartAsync_ReturnsNotFound_WhenCartIsNull()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync((Cart?)null);

			// Act
			var result = await _cartService.RemoveProductFromCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task RemoveProductFromCartAsync_ReturnsNotFound_WhenProductNotInCart()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();
			var cart = new Cart
			{
				Id = cartId,
				CartItems = new List<CartItem>()
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);

			// Act
			var result = await _cartService.RemoveProductFromCartAsync(userId, cartId, productId);

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.NotFound.GetHashCode(), errorResult.StatusCode);
		}

		[Fact]
		public async Task RemoveProductFromCartAsync_RemovesCartItem_WhenCartItemQuantityIsMoreThanOne()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();
			var cartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 3,
				Price = 10m
			};

			var cart = new Cart
			{
				Id = cartId,
				CartItems = new List<CartItem> 
				{ 
					cartItem 
				}
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_cartRepoMock.Setup(x => x.Update(cart));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.RemoveProductFromCartAsync(userId, cartId, productId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
			Assert.Equal(2, cartItem.Quantity);
		}

		[Fact]
		public async Task RemoveProductFromCartAsync_RemovesCartItem_WhenCartItemQuantityIsOne()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var cartId = Guid.NewGuid();
			var productId = Guid.NewGuid();
			var cartItem = new CartItem
			{
				ProductId = productId,
				Quantity = 1,
				Price = 10m
			};

			var cart = new Cart
			{
				Id = cartId,
				CartItems = new List<CartItem> 
				{ 
					cartItem 
				}
			};

			_cartRepoMock.Setup(x => x.GetCartByIdAndUserIdAsync(cartId, userId)).ReturnsAsync(cart);
			_cartRepoMock.Setup(x => x.Update(cart));
			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<bool>())).ReturnsAsync(1);

			// Act
			var result = await _cartService.RemoveProductFromCartAsync(userId, cartId, productId);

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.OK.GetHashCode(), successResult.StatusCode);
			Assert.Empty(cart.CartItems);
		}
	}
}