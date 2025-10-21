using ECommerceApi.Application.Models.Common;
using ECommerceApi.Application.Models.Dtos;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IOrderService
	{
		Task<ICustomResult> CheckoutAsync(Guid userId);
		Task<ICustomResult> GetUserOrdersAsync(Guid userId);
	}
}
