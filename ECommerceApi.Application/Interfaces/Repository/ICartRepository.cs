using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface ICartRepository
	{
		Task CreateAsync(Cart cart);
		Task<Cart?> GetCartByUserIdAsync(Guid userId);
		void UpdateAsync(Cart cart);
		Task<Cart?> GetCartByIdAndUserIdAsync(Guid cartId, Guid userId);
		Task<Cart?> GetByUserIdAndCartIdAsync(Guid userId, Guid cartId);
		Task<Cart?> GetCartWithItemsAndProductsAsync(Guid userId);
	}
}
