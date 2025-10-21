using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class OrderResponse
	{
		public Guid OrderId { get; set; }
	}

	public class OrderResponseExample : IExamplesProvider<SuccessDataResult<OrderResponse>>
	{
		public SuccessDataResult<OrderResponse> GetExamples()
		{
			return new SuccessDataResult<OrderResponse>(new OrderResponse
			{
				OrderId = Guid.NewGuid()
			});
		}
	}
}
