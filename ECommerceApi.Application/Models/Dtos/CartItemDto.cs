using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class CartItemDto
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}

	public class CartItemDtoExample : IExamplesProvider<SuccessDataResult<CartItemDto>>
	{
		public SuccessDataResult<CartItemDto> GetExamples()
		{
			return new SuccessDataResult<CartItemDto>(new CartItemDto
			{
				ProductId = Guid.NewGuid(),
				ProductName = "ABC",
				Quantity = 100,
				Price = 1000m
			});
		}
	}

}
