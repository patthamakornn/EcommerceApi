using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ApplicationDbContext _context;

		public ProductRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			return await _context.Products.ToListAsync();
		}

		public async Task<Product?> GetProductByIdAsync(Guid id)
		{
			return await _context.Products.FindAsync(id);
		}
	}
}
