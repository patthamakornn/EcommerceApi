using ECommerceApi.Application.Interfaces.Repository;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace ECommerceApi.Infrastructure.Repositories
{
	public class CartRepository : ICartRepository
	{
		private readonly ApplicationDbContext _context;

		public CartRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(Cart cart)
		{
			_context.Carts.Add(cart);
			await _context.SaveChangesAsync();
		}

		public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
		{
			return await _context.Carts
			.Include(c => c.CartItems)
			.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(c => c.UserId == userId);
		}

		public async Task<Cart?> GetCartByIdAndUserIdAsync(Guid cartId, Guid userId)
		{
			return await _context.Carts
				.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
				.Where(c => c.Id == cartId && c.UserId == userId)
				.FirstOrDefaultAsync();
		}

		public async Task<Cart?> GetByUserIdAndCartIdAsync(Guid userId, Guid cartId)
		{
			return await _context.Carts
				.Include(c => c.CartItems)
				.Where(c => c.Id == cartId && c.UserId == userId)
				.FirstOrDefaultAsync();
		}

		public void UpdateAsync(Cart cart)
		{
			_context.Carts.Update(cart);
		}

		public async Task<Cart?> GetCartWithItemsAndProductsAsync(Guid userId)
		{
			return await _context.Carts
				.Include(c => c.CartItems)
					.ThenInclude(ci => ci.Product)
				.FirstOrDefaultAsync(c => c.UserId == userId);
		}

	}
}
