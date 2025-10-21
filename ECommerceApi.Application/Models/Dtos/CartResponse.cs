using ECommerceApi.Application.Models.Common;
using Swashbuckle.AspNetCore.Filters;

namespace ECommerceApi.Application.Models.Dtos
{
	public class CartResponse
	{
		public Guid CartId { get; set; }
	}

	public class CartResponseExample : IExamplesProvider<SuccessDataResult<CartResponse>>
	{
		public SuccessDataResult<CartResponse> GetExamples()
		{
			return new SuccessDataResult<CartResponse>(new CartResponse
			{
				CartId = Guid.NewGuid()
			});
		}
	}
}
