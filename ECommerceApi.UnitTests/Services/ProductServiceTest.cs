using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using Moq;
using System.Net;

namespace ECommerceApi.UnitTests.Services
{
	public class ProductServiceTest
	{
		private readonly Mock<IProductRepository> _productRepoMock;
		private readonly ProductService _productService;

		public ProductServiceTest()
		{
			_productRepoMock = new Mock<IProductRepository>();

			_productService = new ProductService(
				_productRepoMock.Object
			);
		}

		[Fact]
		public async Task GetAllProductsAsync_ReturnsSuccessDataResult_WhenProductsExist()
		{   
			// Arrange
			var products = new List<Product>
			{
				new Product { Id = Guid.NewGuid(), ProductName = "Product A", Price = 100 },
				new Product { Id = Guid.NewGuid(), ProductName = "Product B", Price = 200 },
			};

			_productRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

			// Act
			var result = await _productService.GetAllProductsAsync();

			// Assert
			var successResult = Assert.IsType<SuccessDataResult<List<ProductResponse>>>(result);
			Assert.Equal(2, successResult.Data?.Count);
		}

		[Fact]
		public async Task GetAllProductsAsync_ReturnsInternalServerError_WhenException()
		{
			// Arrange
			_productRepoMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("error"));

			// Act
			var result = await _productService.GetAllProductsAsync();

			// Assert
			var errorResult = Assert.IsType<ErrorDataResult<string>>(result);
			Assert.Equal(HttpStatusCode.InternalServerError.GetHashCode(), errorResult.StatusCode);
		}
	}
}