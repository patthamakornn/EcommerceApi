using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class OrderDto
	{
		public Guid OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalAmount { get; set; }
		public List<OrderItemDto> Items { get; set; } = new();
	}

	public class OrderDtoExample : IExamplesProvider<SuccessDataResult<List<OrderDto>>>
	{
		public SuccessDataResult<List<OrderDto>> GetExamples()
		{
			return new SuccessDataResult<List<OrderDto>>(new List<OrderDto>
		{
			new OrderDto
			{
				OrderId = Guid.NewGuid(),
				OrderDate = DateTime.UtcNow,
				TotalAmount = 199.99m,
				Items = new List<OrderItemDto>
				{
					new OrderItemDto
					{
						ProductId = Guid.NewGuid(),
						ProductName = "AA",
						Quantity = 2,
						Price = 499m
					},
					new OrderItemDto
					{
						ProductId = Guid.NewGuid(),
						ProductName = "BB",
						Quantity = 1,
						Price = 999m
					}
				}
			}
		});
		}
	}
}
