using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IProductRepository
	{
		Task<IEnumerable<Product>> GetAllAsync();
		Task<Product?> GetProductByIdAsync(Guid id);
	}
}
