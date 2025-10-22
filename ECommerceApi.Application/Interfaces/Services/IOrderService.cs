using ECommerceApi.Application.Models.Common;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IOrderService
	{
		Task<ICustomResult> CheckoutAsync(Guid userId);
		Task<ICustomResult> GetUserOrdersAsync(Guid userId);
	}
}
