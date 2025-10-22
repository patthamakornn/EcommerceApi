using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Application.Interfaces.Services;
using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;
using Serilog;
using System.Net;

namespace ECommerceApi.Application.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepo;

		public ProductService(IProductRepository productRepo)
		{
			_productRepo = productRepo;
		}

		public async Task<ICustomResult> GetAllProductsAsync()
		{
			try
			{
				var products = await _productRepo.GetAllAsync();

				var productResponses = products.Select(p => new ProductResponse
				{
					ProductId = p.Id,
					ProductName = p.ProductName,
					Description = p.Description,
					Price = p.Price,
					StockQuantity = p.StockQuantity
				}).ToList();

				return new SuccessDataResult<List<ProductResponse>>(productResponses);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "GetAllProductsAsync occurs an error");
				return new ErrorDataResult<string>(HttpStatusCode.InternalServerError, ex.Message);
			}
		}
	}
}
