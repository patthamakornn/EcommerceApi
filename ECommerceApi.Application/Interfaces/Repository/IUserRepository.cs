using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IUserRepository
	{
		Task CreateAsync(User user);
		Task<User?> GetUserByEmailAsync(string email);
	}
}
