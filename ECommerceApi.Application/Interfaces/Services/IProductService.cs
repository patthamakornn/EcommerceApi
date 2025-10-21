using ECommerceApi.Application.Models.Common;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Services
{
	public interface IProductService
	{
		Task<ICustomResult> GetAllProductsAsync();
	}
}
