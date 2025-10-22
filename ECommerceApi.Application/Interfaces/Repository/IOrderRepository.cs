using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IOrderRepository
	{
		void Create(Order order);
		Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
	}
}
