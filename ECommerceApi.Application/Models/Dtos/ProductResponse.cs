using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class ProductResponse
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; set; }
	}

	public class ProductResponseExample : IExamplesProvider<SuccessDataResult<List<ProductResponse>>>
	{
		public SuccessDataResult<List<ProductResponse>> GetExamples()
		{
			return new SuccessDataResult<List<ProductResponse>>(new List<ProductResponse>
		{
			new ProductResponse
			{
				ProductId = Guid.NewGuid(),
				ProductName = "ABC",
				Description = "A-Z",
				Price = 1000m,
				StockQuantity = 100
			},
			new ProductResponse
			{
				ProductId = Guid.NewGuid(),
				ProductName = "DEF",
				Description = "A-Z",
				Price = 1500m,
				StockQuantity = 50
			}
		});
		}
	}
}