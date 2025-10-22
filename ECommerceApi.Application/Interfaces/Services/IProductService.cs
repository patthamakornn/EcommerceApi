using ECommerceApi.Application.Models.Common;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IProductService
	{
		Task<ICustomResult> GetAllProductsAsync();
	}
}
