using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class CartDto
	{
		public Guid CartId { get; set; }
		public List<CartItemDto> Items { get; set; } = new();
		public decimal TotalAmount { get; set; }
	}

	public class CartDtoExample : IExamplesProvider<SuccessDataResult<CartDto>>
	{
		public SuccessDataResult<CartDto> GetExamples()
		{
			return new SuccessDataResult<CartDto>(new CartDto
			{
				CartId = Guid.NewGuid(),
				TotalAmount = 299.97m,
				Items = new List<CartItemDto>
				{
					new CartItemDto
					{
						ProductId = Guid.NewGuid(),
						ProductName = "ABC",
						Quantity = 1,
						Price = 199.99m
					},
					new CartItemDto
					{
						ProductId = Guid.NewGuid(),
						ProductName = "XYZ",
						Quantity = 2,
						Price = 49.99m
					}
				}
			});
		}
	}
}
