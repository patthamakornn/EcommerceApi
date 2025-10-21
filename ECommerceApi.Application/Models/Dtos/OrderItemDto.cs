using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class OrderItemDto
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}

	public class OrderItemDtoExample : IExamplesProvider<SuccessDataResult<OrderItemDto>>
	{
		public SuccessDataResult<OrderItemDto> GetExamples()
		{
			return new SuccessDataResult<OrderItemDto>(new OrderItemDto
			{
				ProductId = Guid.NewGuid(),
				ProductName = "ABC",
				Quantity = 10,
				Price = 999m
			});
		}
	}
}
