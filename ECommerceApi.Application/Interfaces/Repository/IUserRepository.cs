using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces.Repository
{
	public interface IUserRepository
	{
		Task CreateAsync(User user);
		Task<User?> GetUserByEmailAsync(string email);
	}
}
