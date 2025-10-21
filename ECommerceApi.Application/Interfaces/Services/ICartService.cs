using ECommerceApi.Application.Models.Common;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface ICartService
	{
		Task<ICustomResult> CreateCartAsync(Guid userId);
		Task<ICustomResult> AddProductToCartAsync(Guid userId, Guid cartId, Guid productId);
		Task<ICustomResult> GetCartByIdForUserAsync(Guid cartId, Guid userId);
		Task<ICustomResult> RemoveProductFromCartAsync(Guid userId, Guid cartId, Guid productId);
	}
}
