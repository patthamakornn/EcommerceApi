using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IOrderRepository
	{
		void CreateAsync(Order order);
		Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
	}
}
