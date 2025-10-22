using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public void Create(Order order)
		{
			_context.Orders.Add(order);
		}

		public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
		{
			return await _context.Orders
				.Where(o => o.UserId == userId)
				.Include(o => o.OrderItems)
					.ThenInclude(oi => oi.Product)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();
		}
	}
}
